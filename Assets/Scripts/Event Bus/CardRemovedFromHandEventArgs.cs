using System;

public class CardRemovedFromHandEventArgs : EventArgs {
    private MatchPlayer player;
    private Card card;
    private int handIndex;

    public CardRemovedFromHandEventArgs(MatchPlayer player, Card card, int handIndex) {
        this.player = player;
        this.card = card;
        this.handIndex = handIndex;
    }

    public MatchPlayer Player { get { return player; } }

    public Card Card { get { return card; } }

    public int HandIndex { get { return handIndex; } }
}