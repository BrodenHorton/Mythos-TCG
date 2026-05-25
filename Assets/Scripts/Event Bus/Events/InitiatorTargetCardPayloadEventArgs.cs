using System;

public class InitiatorTargetCardPayloadEventArgs<T> : EventArgs where T : CardPayload {
    private ulong initiatorId;
    private ulong targetId;
    private T cardPayload;

    public InitiatorTargetCardPayloadEventArgs(ulong initiatorId, ulong targetId, T cardPayload) {
        this.initiatorId = initiatorId;
        this.targetId = targetId;
        this.cardPayload = cardPayload;
    }

    public ulong InitiatorId { get { return initiatorId; } }

    public ulong TargetId { get { return targetId; } }

    public T CardPayload { get { return cardPayload; } }
}