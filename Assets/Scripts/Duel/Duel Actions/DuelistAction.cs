using System;

public abstract class DuelistAction {
    public event EventHandler OnRemoveAction;

    protected string activeActionMessage;
    protected string inactiveActionMessage;

    public abstract void Execute();

    protected void InvokeOnRemoveAction() {
        OnRemoveAction?.Invoke(this, EventArgs.Empty);
    }

    public void ResetOnRemoveAction() {
        OnRemoveAction = null;
    }

    public string ActiveActionMessage { get { return activeActionMessage; } }

    public string InactiveActionMessage { get { return inactiveActionMessage; } }
}