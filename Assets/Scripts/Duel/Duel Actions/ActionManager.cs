using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class ActionManager : NetworkBehaviour {
    public event EventHandler OnActionAdded;
    public event EventHandler OnActionRemoved;
    public event EventHandler OnCanPerformActionChanged;

    private DuelManager duelManager;
    private Stack<DuelistAction> actions;
    private List<int> actionFocusPlayerIndices; // Only used by the server
    private bool canPerformAction;
    private NetworkVariable<FixedString128Bytes> inactiveActionText = new NetworkVariable<FixedString128Bytes>("");

    private void Awake() {
        actions = new Stack<DuelistAction>();
        actionFocusPlayerIndices = new List<int>();
        canPerformAction = false;
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
        UpdateCanPerformClientRpc(actionFocusPlayerIndices.ToArray());
    }

    [Rpc(SendTo.Server)]
    public void SetActionFocusPlayerIndicesServerRpc(int[] playerIndices) {
        actionFocusPlayerIndices.Clear();
        for(int i = 0; i < playerIndices.Length; i++)
            actionFocusPlayerIndices.Add(playerIndices[i]);
        UpdateCanPerformClientRpc(actionFocusPlayerIndices.ToArray());
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void UpdateCanPerformClientRpc(int[] playerIndices) {
        bool containsPlayerIndex = false;
        for(int i = 0; i < playerIndices.Length; i++) {
            if (playerIndices[i] == duelManager.GetLocalClientPlayerIndex()) {
                containsPlayerIndex = true;
                break;
            }
        }

        canPerformAction = containsPlayerIndex;
        SetInactiveActionText();
        OnCanPerformActionChanged?.Invoke(this, EventArgs.Empty);
    }

    [Rpc(SendTo.Server)]
    public void RemoveActionFocusIndexServerRpc(int playerIndex) {
        if (!actionFocusPlayerIndices.Contains(playerIndex))
            throw new Exception("Attempting to remove player index that does not exist in actionFocusPlayerIndices");

        actionFocusPlayerIndices.Remove(playerIndex);
        UpdateCanPerformClientRpc(actionFocusPlayerIndices.ToArray());
    }

    private void SetInactiveActionText() {
        inactiveActionText.Value = actions.Count > 0 ? actions.Peek().InactiveActionMessage : "";
    }

    public Stack<DuelistAction> Actions { get { return actions; } }

    public List<int> ActionFocusPlayerIndices { get { return actionFocusPlayerIndices; } }

    public bool CanPerformAction { get { return canPerformAction; } }

    public NetworkVariable<FixedString128Bytes> InactiveActionText { get { return inactiveActionText; } }
}
