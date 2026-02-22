using System;

public class DeclareDefenderEventArgs : EventArgs {
    private MatchPlayer defender;
    private CreatureCard card;
    private CreatureCard targetAttacker;

    public DeclareDefenderEventArgs(MatchPlayer defender, CreatureCard card, CreatureCard targetAttacker) {
        this.defender = defender;
        this.card = card;
        this.targetAttacker = targetAttacker;
    }

    public MatchPlayer Defender { get { return defender; } }

    public CreatureCard Card { get { return card; } }

    public CreatureCard TargetAttacker { get { return targetAttacker; } }
}