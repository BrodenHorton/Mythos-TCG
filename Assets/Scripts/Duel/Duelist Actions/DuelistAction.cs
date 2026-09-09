using System;

public abstract class DuelistAction {
    public event EventHandler<PlayerEventArgs> OnRemoveAction;

    protected ulong playerId;
    protected string activeActionMessage;
    protected string inactiveActionMessage;

    public abstract void Execute();

    protected void InvokeOnRemoveAction() {
        OnRemoveAction?.Invoke(this, new PlayerEventArgs(playerId));
    }

    public void ResetOnRemoveAction() {
        OnRemoveAction = null;
    }

    public string ActiveActionMessage { get { return activeActionMessage; } }

    public string InactiveActionMessage { get { return inactiveActionMessage; } }
}