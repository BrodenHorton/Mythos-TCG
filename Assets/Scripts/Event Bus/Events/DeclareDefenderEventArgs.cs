using System;

public class DeclareDefenderEventArgs : EventArgs {
    private MatchPlayer target;
    private CreatureCard defender;
    private CreatureCard attacker;

    public DeclareDefenderEventArgs(MatchPlayer target, CreatureCard defender, CreatureCard attacker) {
        this.target = target;
        this.defender = defender;
        this.attacker = attacker;
    }

    public MatchPlayer Target { get { return target; } }

    public CreatureCard Defender { get { return defender; } }

    public CreatureCard Attacker { get { return attacker; } }
}