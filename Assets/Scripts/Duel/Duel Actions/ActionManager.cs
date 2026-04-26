using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class ActionManager : NetworkBehaviour {
    public event EventHandler OnActionFocusChanged;
    public event EventHandler OnActionAdded;
    public event EventHandler OnActionRemoved;

    private DuelManager duelManager;
    private Stack<DuelistAction> actions;
    private List<int> actionFocusPlayerIndices;
    private List<int> queuedActionFocusPlayerIndices;
    private NetworkVariable<FixedString128Bytes> inactiveActionText = new NetworkVariable<FixedString128Bytes>("");

    private void Awake() {
        actions = new Stack<DuelistAction>();
        actionFocusPlayerIndices = new List<int>();
        queuedActionFocusPlayerIndices = new List<int>();
    }

    private void Start() {
        duelManager = FindFirstObjectByType<DuelManager>();
        if (duelManager == null)
            throw new Exception("Could not find DuelManager object");

        duelManager.OnNextPlayerTurn += (sender, args) => {
            if(IsServer)
                SetActionFocusPlayerIndicesClientRpc(args.PlayerIndex);
        };
        EventBus.OnActionButtonPressed += ExecuteAction;
    }

    private void LateUpdate() {
        if (!IsServer)
            return;

        if(queuedActionFocusPlayerIndices.Count > 0)
            ProcessQueuedActionFocusPlayerIndicesServerRpc();
    }

    public void AddAction(Action callback, string activeActionMessage, string inactiveActionMessage) {
        AddAction(new SimpleDuelistAction(callback, activeActionMessage, inactiveActionMessage));
    }

    public void AddAction(DuelistAction duelistAction) {
        if (actions.Count > 0)
            actions.Peek().ResetOnRemoveAction();
        actions.Push(duelistAction);
        OnActionAdded?.Invoke(this, EventArgs.Empty);
        UpdateOnNewActionAvailable();
    }

    private void ExecuteAction(object sender, EventArgs args) {
        if (actions.Count == 0)
            throw new Exception("Attempting to execute an action when there are no actions on the stack");

        DuelistAction action = actions.Pop();
        action.ResetOnRemoveAction();
        action.Execute();
        OnActionRemoved?.Invoke(this, EventArgs.Empty);
        if (queuedActionFocusPlayerIndices.Count > 0)
            ProcessQueuedActionFocusPlayerIndicesServerRpc();
        else
            UpdateOnNewActionAvailable();
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
        if (queuedActionFocusPlayerIndices.Count > 0)
            ProcessQueuedActionFocusPlayerIndicesServerRpc();
        else
            UpdateOnNewActionAvailable();
    }

    private void UpdateOnNewActionAvailable() {
        if (!actionFocusPlayerIndices.Contains(duelManager.GetLocalClientPlayerIndex()))
            return;

        if (actions.Count != 0)
            actions.Peek().OnRemoveAction += PopAction;
        SetInactiveActionText();
    }

    // Should only be called when you want the action focus to immediately switch to the specified player
    [Rpc(SendTo.ClientsAndHost)]
    public void SetActionFocusPlayerIndicesClientRpc(int playerIndex) {
        actionFocusPlayerIndices.Clear();
        actionFocusPlayerIndices.Add(playerIndex);
        OnActionFocusChanged?.Invoke(this, EventArgs.Empty);
        UpdateOnNewActionAvailable();
    }

    // Should only be called when you want the action focus to immediately switch to the specified players
    [Rpc(SendTo.ClientsAndHost)]
    public void SetActionFocusPlayerIndicesClientRpc(int[] playerIndices) {
        actionFocusPlayerIndices.Clear();
        actionFocusPlayerIndices.AddRange(playerIndices);
        OnActionFocusChanged?.Invoke(this, EventArgs.Empty);
        // Fix this later. Currently will update inactive action text for each player index
        UpdateOnNewActionAvailable();
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void QueueActionFocusPlayerIndicesClientRpc(int playerIndex) {
        queuedActionFocusPlayerIndices.Add(playerIndex);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void QueueActionFocusPlayerIndicesClientRpc(int[] playerIndices) {
        queuedActionFocusPlayerIndices.AddRange(playerIndices);
    }

    [Rpc(SendTo.Server)]
    public void ProcessQueuedActionFocusPlayerIndicesServerRpc() {
        if (queuedActionFocusPlayerIndices.Count == 0)
            throw new Exception("Attempting to process queued action focus player indices when the list is empty");

        int[] playerIndices = queuedActionFocusPlayerIndices.ToArray();
        ClearQueuedActionFocusPlayerIndicesClientRpc();
        SetActionFocusPlayerIndicesClientRpc(playerIndices);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void ClearQueuedActionFocusPlayerIndicesClientRpc() {
        queuedActionFocusPlayerIndices.Clear();
    }

    public void RemoveActionFocusIndex(int playerIndex) {
        if (!actionFocusPlayerIndices.Contains(playerIndex))
            throw new Exception("Attempting to remove player index that does not exist in actionFocusPlayerIndices");

        actionFocusPlayerIndices.Remove(playerIndex);
        OnActionFocusChanged?.Invoke(this, EventArgs.Empty);
    }

    private void SetInactiveActionText() {
        inactiveActionText.Value = actions.Count > 0 ? actions.Peek().InactiveActionMessage : "";
    }

    public Stack<DuelistAction> Actions { get { return actions; } }

    public List<int> ActionFocusPlayerIndices { get { return actionFocusPlayerIndices; } }

    public NetworkVariable<FixedString128Bytes> InactiveActionText { get { return inactiveActionText; } }
}
