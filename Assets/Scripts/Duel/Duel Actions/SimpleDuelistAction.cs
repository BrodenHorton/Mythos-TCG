using System;

public class SimpleDuelistAction : DuelistAction {
    private Action callback;

    public SimpleDuelistAction(Action callback, string activeActionMessage, string inactiveActionMessage) {
        this.callback = callback;
        this.activeActionMessage = activeActionMessage;
        this.inactiveActionMessage = inactiveActionMessage;
    }

    public override void Execute() {
        callback?.Invoke();
    }
}
