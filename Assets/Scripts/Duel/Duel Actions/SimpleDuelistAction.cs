using System;

public class SimpleDuelistAction : DuelistAction {
    private Action callback;
    private string activeActionMessage;
    private string inactiveActionMessage;

    public SimpleDuelistAction(Action callback, string activeActionMessage, string inactiveActionMessage) {
        this.callback = callback;
        this.activeActionMessage = activeActionMessage;
        this.inactiveActionMessage = inactiveActionMessage;
    }

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