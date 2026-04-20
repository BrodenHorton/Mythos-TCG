using System;

public class CallbackAction : DuelAction {
    private Action callback;
    private string activeActionMessage;
    private string inactiveActionMessage;

    public CallbackAction(Action callback, string activeActionMessage, string inactiveActionMessage) {
        this.callback = callback;
        this.activeActionMessage = activeActionMessage;
        this.inactiveActionMessage = inactiveActionMessage;
    }

    public void AddListeners() { }

    public void RemoveListeners() { }

    public void Execute() {
        callback?.Invoke();
    }

    public string ActiveActionMessage() {
        return activeActionMessage;
    }

    public string InactiveActionMessage() {
        return inactiveActionMessage;
    }
}