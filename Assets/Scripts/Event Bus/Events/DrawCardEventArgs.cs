using System;

public class DrawCardEventArgs : EventArgs {
    private MatchPlayer player;
    private Card card;

    public DrawCardEventArgs(MatchPlayer player, Card card) {
        this.player = player;
    }

    public MatchPlayer Player { get { return player; } }

    public Card Card { get { return card; } }
}
