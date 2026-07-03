using System;

public class CreatureCombatEventArgs : EventArgs {
    private ulong initiatorId;
    private ulong targetId;
    private CreatureCard attacker;
    private CreatureCard defender;

    public CreatureCombatEventArgs(ulong initiatorid, ulong targetId, CreatureCombat creatureCombat)
        : this(initiatorid, targetId, creatureCombat.Attacker, creatureCombat.Defender) { }

    public CreatureCombatEventArgs(ulong initiatorId, ulong targetId, CreatureCard attacker, CreatureCard defender) {
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