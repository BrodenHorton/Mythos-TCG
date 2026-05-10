using System;

public class CardRemovedFromHandEventArgs : EventArgs {
    private MatchPlayer player;
    private Card card;

    public CardRemovedFromHandEventArgs(MatchPlayer player, Card card) {
        this.player = player;
        this.card = card;
    }

    public MatchPlayer Player { get { return player; } }

    public Card Card { get { return card; } }
}