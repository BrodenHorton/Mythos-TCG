using System;

public class CreatureAttackEventArgs : EventArgs {
    private MatchPlayer initiator;
    private MatchPlayer target;
    private CreatureCard attacker;
    private CreatureCard defender;

    public CreatureAttackEventArgs(MatchPlayer initiator, MatchPlayer target, CreatureCombat creatureCombat)
        : this(initiator, target, creatureCombat.Attacker, creatureCombat.Defender) {}

    public CreatureAttackEventArgs(MatchPlayer initiator,
                                   MatchPlayer target,
                                   CreatureCard attacker,
                                   CreatureCard defender) {
        this.initiator = initiator;
        this.target = target;
        this.attacker = attacker;
        this.defender = defender;
    }

    public MatchPlayer Initiator { get { return initiator; } }

    public MatchPlayer Target { get { return target; } }

    public CreatureCard Attacker { get { return attacker; } }

    public CreatureCard Defender { get { return defender; } }

}
