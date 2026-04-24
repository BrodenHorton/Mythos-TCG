using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ActionManager : NetworkBehaviour {
    public event EventHandler OnActionFocusChanged;
    public event EventHandler OnActionAdded;
    public event EventHandler OnActionRemoved;
    public event EventHandler<string> OnInactiveActionTextChanged;

    private DuelManager duelManager;
    private Stack<DuelistAction> actions;
    private List<int> actionFocusPlayerIndices;
    private string inactiveActionText;

    private void Awake() {
        actions = new Stack<DuelistAction>();
        actionFocusPlayerIndices = new List<int>();
        inactiveActionText = "";
    }

    private void Start() {
        duelManager = FindFirstObjectByType<DuelManager>();
        if (duelManager == null)
            throw new Exception("Could not find DuelManager object");

        duelManager.OnNextPlayerTurn += (sender, args) => {
            SetActionFocusPlayerIndices(args.PlayerIndex);
        };
        EventBus.OnActionButtonPressed += ExecuteDuelAction;
    }

    public void AddAction(Action callback, string activeActionMessage, string inactiveActionMessage) {
        TcgLogger.Log("Action Added with action message: " + activeActionMessage);
        AddAction(new SimpleDuelistAction(callback, activeActionMessage, inactiveActionMessage));
    }

    public void AddAction(DuelistAction duelAction) {
        actions.Push(duelAction);
        OnActionAdded?.Invoke(this, EventArgs.Empty);
        UpdateInactiveActionText();
    }

    private void ExecuteDuelAction(object sender, EventArgs args) {
        if (actions.Count == 0)
            throw new Exception("Attempting to execute an action when there are no actions on the stack");

        DuelistAction action = actions.Pop();
        action.Execute();
        OnActionRemoved?.Invoke(this, EventArgs.Empty);
        UpdateInactiveActionText();
    }

    private void UpdateInactiveActionText() {
        if (!actionFocusPlayerIndices.Contains(duelManager.GetLocalClientPlayerIndex()))
            return;

        string updatedInvalidActionText = actions.Count > 0 ? actions.Peek().InactiveActionMessage() : "";
        UpdateInactiveActionTextServerRpc(updatedInvalidActionText);
    }

    [Rpc(SendTo.Server)]
    private void UpdateInactiveActionTextServerRpc(string text) {
        UpdateInactiveActionTextClientRpc(text);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void UpdateInactiveActionTextClientRpc(string text) {
        InactiveActionText = text;
    }

    public void SetActionFocusPlayerIndices(int playerIndex) {
        TcgLogger.Log("Action Focus set to player index: " + playerIndex);
        actionFocusPlayerIndices.Clear();
        actionFocusPlayerIndices.Add(playerIndex);
        OnActionFocusChanged?.Invoke(this, EventArgs.Empty);
        UpdateInactiveActionText();
    }

    public void SetActionFocusPlayerIndices(List<int> playerIndices) {
        actionFocusPlayerIndices.Clear();
        actionFocusPlayerIndices.AddRange(playerIndices);
        OnActionFocusChanged?.Invoke(this, EventArgs.Empty);
        UpdateInactiveActionText();
    }

    public void RemoveActionFocusIndex(int playerIndex) {
        if (!actionFocusPlayerIndices.Contains(playerIndex))
            throw new Exception("Attempting to remove player index that does not exist in actionFocusPlayerIndices");

        actionFocusPlayerIndices.Remove(playerIndex);
        OnActionFocusChanged?.Invoke(this, EventArgs.Empty);
        UpdateInactiveActionText();
    }

    public Stack<DuelistAction> Actions { get { return actions; } }

    public List<int> ActionFocusPlayerIndices { get { return actionFocusPlayerIndices; } }

    public string InactiveActionText {
        get {
            return inactiveActionText;
        }
        set {
            inactiveActionText = value;
            OnInactiveActionTextChanged?.Invoke(this, inactiveActionText);
        }
    }
}
