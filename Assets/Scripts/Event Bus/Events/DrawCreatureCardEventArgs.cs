using System;

public class DrawCreatureCardEventArgs : EventArgs {
    private MatchPlayer player;
    private CreatureCard card;

    public DrawCreatureCardEventArgs(MatchPlayer player, CreatureCard card) {
        this.player = player;
        this.card = card;
    }

    public MatchPlayer Player { get { return player; } }

    public CreatureCard Card { get { return card; } }
}
