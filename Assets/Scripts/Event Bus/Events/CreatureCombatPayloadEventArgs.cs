using System;

public class CreatureCombatPayloadEventArgs : EventArgs {
    private ulong initiatorId;
    private ulong targetId;
    private CreatureCardPayload attacker;
    private CreatureCardPayload defender;

    public CreatureCombatPayloadEventArgs(ulong initiatorId, ulong targetId, CreatureCombat creatureCombat)
        : this(initiatorId, targetId, new CreatureCardPayload(creatureCombat.Attacker), new CreatureCardPayload(creatureCombat.Defender)) { }

    public CreatureCombatPayloadEventArgs(ulong initiatorId, ulong targetId, CreatureCard attacker, CreatureCard defender)
        : this (initiatorId, targetId, new CreatureCardPayload(attacker), new CreatureCardPayload(defender)) { }

    public CreatureCombatPayloadEventArgs(ulong initiatorId, ulong targetId, CreatureCardPayload attacker, CreatureCardPayload defender) {
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