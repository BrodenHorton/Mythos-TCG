using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class ActionManager : NetworkBehaviour {
    public event EventHandler OnActionAdded;
    public event EventHandler OnActionRemoved;
    public event EventHandler OnCanPerformActionChanged;
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
        if (actionsByPlayerId[playerId].Count != 0)
            actionsByPlayerId[playerId].Peek().OnRemoveAction += (sender, playerId) => {
                PopAction(playerId);
            };

        if (actionFocusPlayerIds.Contains(playerId))
            SetInactiveActionText();
    }

    public void SetActionFocusPlayerIndices(ulong playerId) {
        if (!IsServer)
            return;

        actionFocusPlayerIds.Clear();
        actionFocusPlayerIds.Add(playerId);
        SetInactiveActionText();
    }

    public void SetActionFocusPlayerIndices(ulong[] playerIds) {
        if (!IsServer)
            return;

        actionFocusPlayerIds.Clear();
        for(int i = 0; i < playerIds.Length; i++)
            actionFocusPlayerIds.Add(playerIds[i]);
        SetInactiveActionText();
    }

    public void RemoveActionFocusId(ulong playerId) {
        if (!IsServer)
            return;
        if (!actionFocusPlayerIds.Contains(playerId))
            throw new Exception("Attempting to remove player index that does not exist in actionFocusPlayerIndices");

        actionFocusPlayerIds.Remove(playerId);
    }

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
