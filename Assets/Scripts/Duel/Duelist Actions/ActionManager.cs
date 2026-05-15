using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class ActionManager : NetworkBehaviour {
    public event EventHandler<ActionStateEventArgs> OnActionStateChanged;

    private DuelManager duelManager;
    private List<ulong> actionFocusPlayerIds;
    private Dictionary<ulong, Stack<DuelistAction>> actionsByPlayerId;

    private void Awake() {
        actionFocusPlayerIds = new List<ulong>();
        actionsByPlayerId = new Dictionary<ulong, Stack<DuelistAction>>();
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

        if (actionsByPlayerId[playerId].Count > 0)
            actionsByPlayerId[playerId].Peek().ResetOnRemoveAction();
        duelistAction.OnRemoveAction += (sender, playerId) => {
            PopAction(playerId);
        };
        actionsByPlayerId[playerId].Push(duelistAction);

        if(actionFocusPlayerIds.Contains(playerId))
            UpdateAllClientsActionState();
    }

    [Rpc(SendTo.Server)]
    public void ExecuteActionServerRpc(RpcParams rpcParams = default) {
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
        if(actionFocusPlayerIds.Contains(playerId))
            UpdateAllClientsActionState();
    }

    public void PopAction(ulong playerId) {
        if (!IsServer)
            return;
        if (actionsByPlayerId.Count == 0)
            throw new Exception("Attempting to remove an action when the action stack is empty");

        DuelistAction action = actionsByPlayerId[playerId].Pop();
        action.ResetOnRemoveAction();
        if (actionFocusPlayerIds.Contains(playerId))
            UpdateAllClientsActionState();
    }

    public void SetActionFocusPlayerIndices(ulong playerId) {
        if (!IsServer)
            return;

        actionFocusPlayerIds.Clear();
        actionFocusPlayerIds.Add(playerId);
        UpdateAllClientsActionState();
    }

    public void SetActionFocusPlayerIndices(ulong[] playerIds) {
        if (!IsServer)
            return;

        actionFocusPlayerIds.Clear();
        for(int i = 0; i < playerIds.Length; i++)
            actionFocusPlayerIds.Add(playerIds[i]);
        UpdateAllClientsActionState();
    }

    public void RemoveActionFocusId(ulong playerId) {
        if (!IsServer)
            return;
        if (!actionFocusPlayerIds.Contains(playerId))
            throw new Exception("Attempting to remove player index that does not exist in actionFocusPlayerIndices");

        actionFocusPlayerIds.Remove(playerId);
        UpdateAllClientsActionState();
    }

    private void UpdateAllClientsActionState() {
        List<ulong> inactivePlayerIds = new List<ulong>();
        for (int i = 0; i < duelManager.Players.Count; i++) {
            if (!actionFocusPlayerIds.Contains(duelManager.Players[i].PlayerId))
                inactivePlayerIds.Add(duelManager.Players[i].PlayerId);
        }

        string activeActionMessage;
        string inactiveActionMessage;
        if (actionFocusPlayerIds.Count == 0) {
            activeActionMessage = "";
            inactiveActionMessage = "";
        }
        else {
            activeActionMessage = actionsByPlayerId[actionFocusPlayerIds[0]].Count > 0 ? actionsByPlayerId[actionFocusPlayerIds[0]].Peek().ActiveActionMessage : "";
            inactiveActionMessage = actionsByPlayerId[actionFocusPlayerIds[0]].Count > 0 ? actionsByPlayerId[actionFocusPlayerIds[0]].Peek().InactiveActionMessage : "";
        }

        BaseRpcTarget activeTarget = RpcTarget.Group(actionFocusPlayerIds, RpcTargetUse.Temp);
        InvokeOnActiveActionChangedClientRpc(true, activeActionMessage, activeTarget);

        BaseRpcTarget inactiveTarget = RpcTarget.Group(inactivePlayerIds, RpcTargetUse.Temp);
        InvokeOnActiveActionChangedClientRpc(false, inactiveActionMessage, inactiveTarget);
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void InvokeOnActiveActionChangedClientRpc(bool hasActionFocus, FixedString128Bytes actionMessage, RpcParams rpcParams) {
        OnActionStateChanged?.Invoke(this, new ActionStateEventArgs(hasActionFocus, actionMessage.ToString()));
    }

    public List<ulong> ActionFocusPlayerIds { get { return actionFocusPlayerIds; } }

    public Dictionary<ulong,Stack<DuelistAction>> ActionsByPlayerId { get { return actionsByPlayerId; } }
}
