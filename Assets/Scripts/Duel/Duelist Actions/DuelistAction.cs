using System;

public abstract class DuelistAction {
    public event EventHandler<ulong> OnRemoveAction;

    protected ulong playerId;
    protected string activeActionMessage;
    protected string inactiveActionMessage;

    public abstract void Execute();

    protected void InvokeOnRemoveAction() {
        OnRemoveAction?.Invoke(this, playerId);
    }

    public void ResetOnRemoveAction() {
        OnRemoveAction = null;
    }

    public string ActiveActionMessage { get { return activeActionMessage; } }

    public string InactiveActionMessage { get { return inactiveActionMessage; } }
}