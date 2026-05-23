using System;

public class UndeclareDefenderPayloadEventArgs : EventArgs {
    private ulong initiatorId;
    private ulong targetId;
    private CreatureCardPayload defender;

    public UndeclareDefenderPayloadEventArgs(ulong initiatorId, ulong targetId, CreatureCardPayload defender) {
        this.initiatorId = initiatorId;
        this.targetId = targetId;
        this.defender = defender;
    }

    public ulong InitiatorId { get { return initiatorId; } }

    public ulong TargetId { get { return targetId; } }

    public CreatureCardPayload Defender { get { return defender; } }
}