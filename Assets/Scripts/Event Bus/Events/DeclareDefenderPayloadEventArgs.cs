using System;

public class DeclareDefenderPayloadEventArgs : EventArgs {
    private ulong initiatorId;
    private ulong targetId;
    private CreatureCardPayload attacker;
    private CreatureCardPayload defender;

    public DeclareDefenderPayloadEventArgs(ulong initiatorId, ulong targetId, CreatureCardPayload attacker, CreatureCardPayload defender) {
        this.initiatorId = initiatorId;
        this.targetId = targetId;
        this.attacker = attacker;
        this.defender = defender;
    }

    public ulong InitiatorId { get { return initiatorId; } }

    public ulong TargetId { get { return targetId; } }

    public CreatureCardPayload Attacker { get { return attacker; } }

    public CreatureCardPayload Defender { get { return defender; } }
}