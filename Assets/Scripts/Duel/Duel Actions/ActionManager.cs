using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class ActionManager : NetworkBehaviour {
    public event EventHandler<List<ulong>> OnActionFocusChanged;
    public event EventHandler<DuelistActionMessagesEventArgs> OnActiveActionChanged;
    public event EventHandler<string> OnInactiveActionTextChanged;

    private DuelManager duelManager;
    private List<ulong> actionFocusPlayerIds;
    private Dictionary<ulong, Stack<DuelistAction>> actionsByPlayerId;
    private FixedString128Bytes inactiveActionText;

    private void Awake() {
        actionFocusPlayerIds = new List<ulong>();
        actionsByPlayerId = new Dictionary<ulong, Stack<DuelistAction>>();
        inactiveActionText = "";
    }

    private void Start() {
        if (!IsServer)
            return;

        duelManager = FindFirstObjectByType<DuelManager>();
        if (duelManager == null)
            throw new Exception("Could not find DuelManager object");

        duelManager.OnPlayersInitialization += InitializeActionManager;
        duelManager.OnNextPlayerTurn += (sender, args) => {
            if(IsServer)
                SetActionFocusPlayerIndices(args.Player.PlayerId);
        };
    }

    private void InitializeActionManager(object sender, PlayersInitializedEventArgs args) {
        if (!IsServer)
            return;

        for(int i = 0; i < args.PlayerOrder.Count; i++)
            actionsByPlayerId.Add(args.PlayerOrder[i], new Stack<DuelistAction>());
    }

    public void AddAction(ulong playerId, Action callback, string activeActionMessage, string inactiveActionMessage) {
        if (!IsServer)
            return;

        AddAction(playerId, new SimpleDuelistAction(playerId, callback, activeActionMessage, inactiveActionMessage));
    }

    public void AddAction(ulong playerId, DuelistAction duelistAction) {
        if (!IsServer)
            return;

        if (actionsByPlayerId.Count > 0)
            actionsByPlayerId[playerId].Peek().ResetOnRemoveAction();
        actionsByPlayerId[playerId].Push(duelistAction);
        OnActionAdded?.Invoke(this, EventArgs.Empty);
        UpdateNewActionAvailable(playerId);
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void InvokeOnActionAddedClientRpc(string actionMessage, string inactiveActionMessage, RpcParams rpcParams) {
        OnActionAdded?.Invoke(this, new DuelistActionMessagesEventArgs(actionMessage, inactiveActionMessage));
    }

    [Rpc(SendTo.Server)]
    public void ExecuteActionServerRpc(ServerRpcParams rpcParams = default) {
        if (!IsServer)
            return;
        ulong playerId = rpcParams.Receive.SenderClientId;
        if (!actionFocusPlayerIds.Contains(playerId))
            throw new Exception("Attempting to execute action for playerId that is not currently in actionFocusPlayerIds: " + playerId);
        if (actionsByPlayerId.Count == 0)
            throw new Exception("Attempting to execute an action when there are no actions on the stack");

        DuelistAction action = actionsByPlayerId[playerId].Pop();
        action.ResetOnRemoveAction();
        action.Execute();
        OnActionRemoved?.Invoke(this, EventArgs.Empty);
        UpdateNewActionAvailable(playerId);
    }

    public void PopAction(ulong playerId) {
        if (!IsServer)
            return;
        if (actionsByPlayerId.Count == 0)
            throw new Exception("Attempting to remove an action when the action stack is empty");

        DuelistAction action = actionsByPlayerId[playerId].Pop();
        action.ResetOnRemoveAction();
        OnActionRemoved?.Invoke(this, EventArgs.Empty);
        UpdateNewActionAvailable(playerId);
    }

    private void UpdateNewActionAvailable(ulong playerId) {
        if (!IsServer)
            return;

        if (actionsByPlayerId[playerId].Count != 0) {
            actionsByPlayerId[playerId].Peek().OnRemoveAction += (sender, playerId) => {
                PopAction(playerId);
            };
        }

        if (actionFocusPlayerIds.Contains(playerId))
            SetInactiveActionText();
    }

    public void SetActionFocusPlayerIndices(ulong playerId) {
        if (!IsServer)
            return;

        actionFocusPlayerIds.Clear();
        actionFocusPlayerIds.Add(playerId);
        List<ulong> inactivePlayerIds = new List<ulong>();
        for(int i = 0; i < duelManager.Players.Count; i++) {
            if (duelManager.Players[i].PlayerId != playerId)
                inactivePlayerIds.Add(duelManager.Players[i].PlayerId);
        }
        string actionButtonMessage = actionsByPlayerId[playerId].Count > 0 ? actionsByPlayerId[playerId].Peek().ActiveActionMessage : "";
        BaseRpcTarget activeTarget = RpcTarget.Group(actionFocusPlayerIds, RpcTargetUse.Temp);
        InvokeOnActionFocusChanged(true, actionButtonMessage, activeTarget);
        BaseRpcTarget inactiveTarget = RpcTarget.Group(inactivePlayerIds, RpcTargetUse.Temp);
        InvokeOnActionFocusChanged(false, "", activeTarget);
        SetInactiveActionText();
    }

    public void SetActionFocusPlayerIndices(ulong[] playerIds) {
        if (!IsServer)
            return;

        actionFocusPlayerIds.Clear();
        for(int i = 0; i < playerIds.Length; i++)
            actionFocusPlayerIds.Add(playerIds[i]);
        InvokeOnActionFocusChanged(actionFocusPlayerIds.ToArray());
        SetInactiveActionText();
    }

    public void RemoveActionFocusId(ulong playerId) {
        if (!IsServer)
            return;
        if (!actionFocusPlayerIds.Contains(playerId))
            throw new Exception("Attempting to remove player index that does not exist in actionFocusPlayerIndices");

        actionFocusPlayerIds.Remove(playerId);
        InvokeOnActionFocusChanged(actionFocusPlayerIds.ToArray());
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void InvokeOnActionFocusChanged(bool isActive, string actionButtonMessage, RpcParams rpcParams) {
        
        OnActionFocusChanged?.Invoke(this, new List<ulong>(actionFocusPlayerIdsArr));
    }

    // TODO: Make it so you can set teh inactive action text without invoking the client rpc
    private void SetInactiveActionText() {
        if (!IsServer)
            return;

        inactiveActionText = actionFocusPlayerIds.Count > 0 ? actionsByPlayerId[actionFocusPlayerIds[0]].Peek().InactiveActionMessage : "";
        InvokeOnInactiveActionTextChangedClientRpc(inactiveActionText);
    }

    [Rpc(SendTo.ClientsAndHost)] 
    private void InvokeOnInactiveActionTextChangedClientRpc(FixedString128Bytes inactiveActionMessage) {
        OnInactiveActionTextChanged?.Invoke(this, inactiveActionMessage.ToString());
    }

    public List<ulong> ActionFocusPlayerIds { get { return actionFocusPlayerIds; } }

    public Dictionary<ulong,Stack<DuelistAction>> ActionsByPlayerId { get { return actionsByPlayerId; } }

    public FixedString128Bytes InactiveActionText { get { return inactiveActionText; } }
}

public class DuelistActionMessagesEventArgs : EventArgs {
    private string actionMessage;
    private string inactiveActionMessage;

    public DuelistActionMessagesEventArgs(string actionMessage, string inactiveActionMessage) {
        this.actionMessage = actionMessage;
        this.inactiveActionMessage = inactiveActionMessage;
    }

    public string ActionMessage { get { return actionMessage; } }

    public string InactiveActionMessage { get { return inactiveActionMessage; } }
}