using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class ActionManager : NetworkBehaviour {
    public event EventHandler OnActionAdded;
    public event EventHandler OnActionRemoved;

    private DuelManager duelManager;
    private Stack<DuelistAction> actions;
    private NetworkList<int> actionFocusPlayerIndices = new NetworkList<int>();
    private NetworkVariable<FixedString128Bytes> inactiveActionText = new NetworkVariable<FixedString128Bytes>("");

    private void Awake() {
        actions = new Stack<DuelistAction>();

        actionFocusPlayerIndices.OnListChanged += (changedEvent) => {
            SetInactiveActionText();
        };
    }

    private void Start() {
        duelManager = FindFirstObjectByType<DuelManager>();
        if (duelManager == null)
            throw new Exception("Could not find DuelManager object");

        duelManager.OnNextPlayerTurn += (sender, args) => {
            if(IsServer)
                SetActionFocusPlayerIndices(args.PlayerIndex);
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
        TcgLogger.Log("UpdatingOnNewActionAvailable");
        if (actions.Count != 0)
            actions.Peek().OnRemoveAction += PopAction;

        if (!actionFocusPlayerIndices.Contains(duelManager.GetLocalClientPlayerIndex()))
            SetInactiveActionText();
    }

    public void SetActionFocusPlayerIndices(int playerIndex) {
        actionFocusPlayerIndices.Clear();
        actionFocusPlayerIndices.Add(playerIndex);
        TcgLogger.Log("Set action focus to: " + playerIndex);
        TcgLogger.Log("Current action focus index: " + actionFocusPlayerIndices[0]);
    }

    public void SetActionFocusPlayerIndices(int[] playerIndices) {
        actionFocusPlayerIndices.Clear();
        for(int i = 0; i < playerIndices.Length; i++)
            actionFocusPlayerIndices.Add(playerIndices[i]);
    }

    public void RemoveActionFocusIndex(int playerIndex) {
        if (!actionFocusPlayerIndices.Contains(playerIndex))
            throw new Exception("Attempting to remove player index that does not exist in actionFocusPlayerIndices");

        actionFocusPlayerIndices.Remove(playerIndex);
    }

    private void SetInactiveActionText() {
        inactiveActionText.Value = actions.Count > 0 ? actions.Peek().InactiveActionMessage : "";
    }

    public Stack<DuelistAction> Actions { get { return actions; } }

    public NetworkList<int> ActionFocusPlayerIndices { get { return actionFocusPlayerIndices; } }

    public NetworkVariable<FixedString128Bytes> InactiveActionText { get { return inactiveActionText; } }
}
