using System;

public class UndeclareAttackerEventArgs : EventArgs {
    private ulong initiatorId;
    private ulong targetId;
    private CreatureCard attacker;

    public UndeclareAttackerEventArgs(ulong initiatorId, ulong targetId, CreatureCard attacker) {
        this.initiatorId = initiatorId;
        this.targetId = targetId;
        this.attacker = attacker;
    }

    public ulong InitiatorId { get { return initiatorId; } }

    public ulong TargetId { get { return targetId; } }

    public CreatureCard Attacker { get { return attacker; } }
}
