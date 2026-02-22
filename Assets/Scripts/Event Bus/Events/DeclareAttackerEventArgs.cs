using System;

public class DeclareAttackerEventArgs : EventArgs {
    private MatchPlayer attacker;
    private MatchPlayer defender;
    private CreatureCard card;

    public DeclareAttackerEventArgs(MatchPlayer attacker, MatchPlayer defender, CreatureCard card) {
        this.attacker = attacker;
        this.defender = defender;
        this.card = card;
    }

    public MatchPlayer Attacker { get { return attacker; } }

    public MatchPlayer Defender { get { return defender; } }

    public CreatureCard Card { get { return card; } }
}
