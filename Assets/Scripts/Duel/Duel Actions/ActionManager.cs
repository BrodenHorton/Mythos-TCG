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
    private NetworkVariable<FixedString128Bytes> inactiveActionText = new NetworkVariable<FixedString128Bytes>("");

    private void Awake() {
        actions = new Stack<DuelistAction>();
        actionFocusPlayerIndices = new List<int>();
    }

    private void Start() {
        duelManager = FindFirstObjectByType<DuelManager>();
        if (duelManager == null)
            throw new Exception("Could not find DuelManager object");

        duelManager.OnNextPlayerTurn += (sender, args) => {
            SetActionFocusPlayerIndices(args.PlayerIndex);
        };
        EventBus.OnActionButtonPressed += ExecuteDuelistAction;
    }

    public void AddAction(Action callback, string activeActionMessage, string inactiveActionMessage) {
        AddAction(new SimpleDuelistAction(callback, activeActionMessage, inactiveActionMessage));
    }

    public void AddAction(DuelistAction duelAction) {
        actions.Push(duelAction);
        OnActionAdded?.Invoke(this, EventArgs.Empty);
        if (actionFocusPlayerIndices.Contains(duelManager.GetLocalClientPlayerIndex()))
            SetInactiveActionText();
    }

    private void ExecuteDuelistAction(object sender, EventArgs args) {
        if (actions.Count == 0)
            throw new Exception("Attempting to execute an action when there are no actions on the stack");

        DuelistAction action = actions.Pop();
        action.Execute();
        OnActionRemoved?.Invoke(this, EventArgs.Empty);
        if (actionFocusPlayerIndices.Contains(duelManager.GetLocalClientPlayerIndex()))
            SetInactiveActionText();
    }

    public void PopAction() {
        if (actions.Count == 0)
            throw new Exception("Attempting to remove an action when the action stack is empty");

        actions.Pop();
        OnActionRemoved?.Invoke(this, EventArgs.Empty);
        if (actionFocusPlayerIndices.Contains(duelManager.GetLocalClientPlayerIndex()))
            SetInactiveActionText();
    }

    public void SetActionFocusPlayerIndices(int playerIndex) {
        actionFocusPlayerIndices.Clear();
        actionFocusPlayerIndices.Add(playerIndex);
        OnActionFocusChanged?.Invoke(this, EventArgs.Empty);

        if (actionFocusPlayerIndices.Contains(duelManager.GetLocalClientPlayerIndex()))
            SetInactiveActionText();
    }

    public void SetActionFocusPlayerIndices(List<int> playerIndices) {
        actionFocusPlayerIndices.Clear();
        actionFocusPlayerIndices.AddRange(playerIndices);
        OnActionFocusChanged?.Invoke(this, EventArgs.Empty);

        // Fix this later. Currently will update inactive action text for each player index
        if (actionFocusPlayerIndices.Contains(duelManager.GetLocalClientPlayerIndex()))
            SetInactiveActionText();
    }

    public void RemoveActionFocusIndex(int playerIndex) {
        if (!actionFocusPlayerIndices.Contains(playerIndex))
            throw new Exception("Attempting to remove player index that does not exist in actionFocusPlayerIndices");

        actionFocusPlayerIndices.Remove(playerIndex);
        OnActionFocusChanged?.Invoke(this, EventArgs.Empty);
    }

    private void SetInactiveActionText() {
        inactiveActionText.Value = actions.Count > 0 ? actions.Peek().InactiveActionMessage() : "";
    }

    public Stack<DuelistAction> Actions { get { return actions; } }

    public List<int> ActionFocusPlayerIndices { get { return actionFocusPlayerIndices; } }

    public NetworkVariable<FixedString128Bytes> InactiveActionText { get { return inactiveActionText; } }
}
