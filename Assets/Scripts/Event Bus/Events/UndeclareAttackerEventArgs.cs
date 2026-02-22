using System;

public class UndeclareAttackerEventArgs : EventArgs {
    private MatchPlayer attacker;
    private CreatureCard card;

    public UndeclareAttackerEventArgs(MatchPlayer attacker, CreatureCard card) {
        this.attacker = attacker;
        this.card = card;
    }

    public MatchPlayer Attacker { get { return attacker; } }

    public CreatureCard Card { get { return card; } }
}