using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ActionManager : NetworkBehaviour {
    public event EventHandler<int> OnActionFocusChanged;
    public event EventHandler<bool> OnCanPerformActionChanged;
    public event EventHandler<string> OnInactiveActionTextChanged;

    private Stack<DuelAction> actions;
    private int actionFocusPlayerIndex;
    private bool canPerformAction;
    private string inactiveActionText;

    private void Awake() {
        actions = new Stack<DuelAction>();
        actionFocusPlayerIndex = 0;
        inactiveActionText = "";
    }

    private void Start() {
        DuelManager duelManager = FindFirstObjectByType<DuelManager>();
        if (duelManager == null)
            throw new Exception("Could not find DuelManager object");

        duelManager.OnNextPlayerTurn += (sender, args) => {
            ActionFocusPlayerIndex = args.PlayerIndex;
        };
        EventBus.OnActionButtonPressed += ExecuteDuelAction;
    }

    public void AddAction(Action callback, string activeActionMessage, string inactiveActionMessage) {
        AddAction(new CallbackAction(callback, activeActionMessage, inactiveActionMessage));
    }

    public void AddAction(DuelAction duelAction) {
        actions.Push(duelAction);
    }

    public void SetCanPerformAction(bool canPerformAction) {
        this.canPerformAction = canPerformAction;
        OnCanPerformActionChanged?.Invoke(this, canPerformAction);
        if (this.canPerformAction)
            UpdateInactiveActionTextServerRpc(actions.Peek().InactiveActionMessage());
    }

    private void ExecuteDuelAction(object sender, EventArgs args) {
        SetCanPerformAction(false);
        DuelAction action = actions.Pop();
        action.Execute();
    }

    [Rpc(SendTo.Server)]
    private void UpdateInactiveActionTextServerRpc(string text) {
        UpdateInactiveActionTextClientRpc(text);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void UpdateInactiveActionTextClientRpc(string text) {
        InactiveActionText = text;
    }

    public Stack<DuelAction> Actions { get { return actions; } }

    public int ActionFocusPlayerIndex {
        get {
            return actionFocusPlayerIndex;
        }
        set {
            if (actionFocusPlayerIndex == value)
                throw new Exception("Attempting to set the action focus to the player who already has the action focus");

            actionFocusPlayerIndex = value;
            OnActionFocusChanged?.Invoke(this, actionFocusPlayerIndex);
        }
    }

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
