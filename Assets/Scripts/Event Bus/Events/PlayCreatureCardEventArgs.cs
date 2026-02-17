using System;

public class PlayCreatureCardEventArgs : EventArgs {
    private MatchPlayer player;
    private CreatureCard card;

    public PlayCreatureCardEventArgs(MatchPlayer player, CreatureCard card) {
        this.player = player;
        this.card = card;
    }

    public MatchPlayer Player { get { return player; } }

    public CreatureCard Card { get { return card; } }
}
