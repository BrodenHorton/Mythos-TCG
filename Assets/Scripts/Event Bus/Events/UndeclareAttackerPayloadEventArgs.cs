using System;

public class UndeclareAttackerPayloadEventArgs : EventArgs {
    private ulong initiatorId;
    private ulong targetId;
    private CreatureCardPayload attacker;

    public UndeclareAttackerPayloadEventArgs(ulong initiatorId, ulong targetId, CreatureCardPayload attacker) {
        this.initiatorId = initiatorId;
        this.targetId = targetId;
        this.attacker = attacker;
    }

    public ulong InitiatorId { get { return initiatorId; } }

    public ulong TargetId { get { return targetId; } }

    public CreatureCardPayload Attacker { get { return attacker; } }
}