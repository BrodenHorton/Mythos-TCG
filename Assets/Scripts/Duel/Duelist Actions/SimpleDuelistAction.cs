using System;

public class SimpleDuelistAction : DuelistAction {
    private Action<ulong> callback;

    public SimpleDuelistAction(ulong playerId, Action<ulong> callback, string activeActionMessage, string inactiveActionMessage) {
        this.playerId = playerId;
        this.callback = callback;
        this.activeActionMessage = activeActionMessage;
        this.inactiveActionMessage = inactiveActionMessage;
    }

    public override void Execute() {
        callback?.Invoke(playerId);
    }
}
