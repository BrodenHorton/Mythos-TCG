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
    private Stack<DuelistAction> actions;
    private bool canPerformAction;

    // Server Fields
    private List<int> actionFocusPlayerIndices;
    private FixedString128Bytes inactiveActionText;

    private void Awake() {
        actions = new Stack<DuelistAction>();
        canPerformAction = false;
        actionFocusPlayerIndices = new List<int>();
        inactiveActionText = "";
    }

    private void Start() {
        duelManager = FindFirstObjectByType<DuelManager>();
        if (duelManager == null)
            throw new Exception("Could not find DuelManager object");

        duelManager.OnNextPlayerTurn += (sender, args) => {
            if(IsServer)
                SetActionFocusPlayerIndicesServerRpc(args.PlayerIndex);
        };
        EventBus.OnActionButtonPressed += ExecuteAction;
    }

    public void AddAction(Action callback, string activeActionMessage, string inactiveActionMessage) {
        AddAction(new SimpleDuelistAction(callback, activeActionMessage, inactiveActionMessage));
    }

    public void AddAction(DuelistAction duelistAction) {
        if (actions.Count > 0)
            actions.Peek().ResetOnRemoveAction();
        actions.Push(duelistAction);
        OnActionAdded?.Invoke(this, EventArgs.Empty);
        UpdateNewActionAvailable();
    }

    private void ExecuteAction(object sender, EventArgs args) {
        if (actions.Count == 0)
            throw new Exception("Attempting to execute an action when there are no actions on the stack");
        if(!canPerformAction)
            throw new Exception("Attempting to execute an action when canPerformAction is false");

        DuelistAction action = actions.Pop();
        action.ResetOnRemoveAction();
        action.Execute();
        OnActionRemoved?.Invoke(this, EventArgs.Empty);
        UpdateNewActionAvailable();
    }

    public void PopAction(object sender, EventArgs args) {
        PopAction();
    }

    public void PopAction() {
        if (actions.Count == 0)
            throw new Exception("Attempting to remove an action when the action stack is empty");

        DuelistAction action = actions.Pop();
        action.ResetOnRemoveAction();
        OnActionRemoved?.Invoke(this, EventArgs.Empty);
        UpdateNewActionAvailable();
    }

    private void UpdateNewActionAvailable() {
        if (actions.Count != 0)
            actions.Peek().OnRemoveAction += PopAction;

        if (canPerformAction)
            SetInactiveActionText();
    }

    [Rpc(SendTo.Server)]
    public void SetActionFocusPlayerIndicesServerRpc(int playerIndex) {
        actionFocusPlayerIndices.Clear();
        actionFocusPlayerIndices.Add(playerIndex);
        UpdateCanPerformActionClientRpc(actionFocusPlayerIndices.ToArray());
        UpdateInactiveActionTextServerRpc();
    }

    [Rpc(SendTo.Server)]
    public void SetActionFocusPlayerIndicesServerRpc(int[] playerIndices) {
        actionFocusPlayerIndices.Clear();
        for(int i = 0; i < playerIndices.Length; i++)
            actionFocusPlayerIndices.Add(playerIndices[i]);
        UpdateCanPerformActionClientRpc(actionFocusPlayerIndices.ToArray());
        UpdateInactiveActionTextServerRpc();
    }

    [Rpc(SendTo.Server)]
    public void RemoveActionFocusIndexServerRpc(int playerIndex) {
        if (!actionFocusPlayerIndices.Contains(playerIndex))
            throw new Exception("Attempting to remove player index that does not exist in actionFocusPlayerIndices");

        actionFocusPlayerIndices.Remove(playerIndex);
        UpdateCanPerformActionClientRpc(actionFocusPlayerIndices.ToArray());
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void UpdateCanPerformActionClientRpc(int[] playerIndices) {
        bool containsPlayerIndex = false;
        for(int i = 0; i < playerIndices.Length; i++) {
            if (playerIndices[i] == duelManager.GetLocalClientPlayerIndex()) {
                containsPlayerIndex = true;
                break;
            }
        }

        canPerformAction = containsPlayerIndex;
        OnCanPerformActionChanged?.Invoke(this, EventArgs.Empty);
    }

    [Rpc(SendTo.Server)]
    private void UpdateInactiveActionTextServerRpc() {
        List<ulong> actionFocusPlayerIds = new List<ulong>();
        for(int i = 0; i < actionFocusPlayerIndices.Count; i++)
            actionFocusPlayerIds.Add(duelManager.Players[actionFocusPlayerIndices[i]].PlayerId);
        BaseRpcTarget rpcTarget = RpcTarget.Group(actionFocusPlayerIds, RpcTargetUse.Temp);
        UpdateInactiveActionTextClientRpc(rpcTarget);
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void UpdateInactiveActionTextClientRpc(RpcParams rpcParams) {
        if (!canPerformAction)
            throw new Exception("Attmepting to update inactive action text when canPerformAction is false");

        SetInactiveActionText();
    }

    private void SetInactiveActionText() {
        string inactiveActionMessage = actions.Count > 0 ? actions.Peek().InactiveActionMessage : "";
        SetInactiveActionTextServerRpc(duelManager.GetLocalClientPlayerIndex(), inactiveActionMessage);
    }

    [Rpc(SendTo.Server)]
    private void SetInactiveActionTextServerRpc(int senderIndex, FixedString128Bytes message) {
        if (!actionFocusPlayerIndices.Contains(senderIndex))
            return;

        inactiveActionText = message;
        InvokeOnInactiveActionTextChangedClientRpc(senderIndex, inactiveActionText);
    }

    [Rpc(SendTo.ClientsAndHost)] 
    private void InvokeOnInactiveActionTextChangedClientRpc(int senderIndex, FixedString128Bytes inactiveActionMessage) {
        OnInactiveActionTextChanged?.Invoke(this, inactiveActionMessage.ToString());
    }

    public Stack<DuelistAction> Actions { get { return actions; } }

    public List<int> ActionFocusPlayerIndices { get { return actionFocusPlayerIndices; } }

    public bool CanPerformAction { get { return canPerformAction; } }

    public FixedString128Bytes InactiveActionText { get { return inactiveActionText; } }
}
