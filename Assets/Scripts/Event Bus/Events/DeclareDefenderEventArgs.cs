using System;

public class DeclareDefenderEventArgs : EventArgs {
    private ulong initiatorId;
    private ulong targetId;
    private CreatureCard attacker;
    private CreatureCard defender;

    public DeclareDefenderEventArgs(ulong initiatorId, ulong targetId, CreatureCard attacker, CreatureCard defender) {
        this.initiatorId = initiatorId;
        this.targetId = targetId;
        this.attacker = attacker;
        this.defender = defender;
    }

    public ulong InitiatorId { get { return initiatorId; } }

    public ulong TargetId { get { return targetId; } }
    
    public CreatureCard Attacker { get { return attacker; } }

    public CreatureCard Defender { get { return defender; } }
}
