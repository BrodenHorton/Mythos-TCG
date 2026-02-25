using System;

public class UndeclareAttackerEventArgs : EventArgs {
    private MatchPlayer initiator;
    private MatchPlayer target;
    private CreatureCard attacker;

    public UndeclareAttackerEventArgs(MatchPlayer initiator, MatchPlayer target, CreatureCard attacker) {
        this.initiator = initiator;
        this.target = target;
        this.attacker = attacker;
    }

    public MatchPlayer Initiator { get { return initiator; } }

    public MatchPlayer Target { get { return target; } }

    public CreatureCard Attacker { get { return attacker; } }
}
