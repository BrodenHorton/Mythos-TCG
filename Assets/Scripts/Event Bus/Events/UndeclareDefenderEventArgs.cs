using System;

public class UndeclareDefenderEventArgs : EventArgs {
    private MatchPlayer initiator;
    private MatchPlayer target;
    private CreatureCard defender;

    public UndeclareDefenderEventArgs(MatchPlayer initiator, MatchPlayer target, CreatureCard defender) {
        this.initiator = initiator;
        this.target = target;
        this.defender = defender;
    }

    public MatchPlayer Initiator { get { return initiator; } }

    public MatchPlayer Target { get { return target; } }

    public CreatureCard Defender { get { return defender; } }
}
