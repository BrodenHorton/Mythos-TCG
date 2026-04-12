using System;

public class PlayCardFromHandEventArgs<T> : EventArgs where T : Card {
    private MatchPlayer player;
    private T card;
    private int handIndex;

    public PlayCardFromHandEventArgs(MatchPlayer player, T card, int handIndex) {
        this.player = player;
        this.card = card;
        this.handIndex = handIndex;
    }

    public MatchPlayer Player { get { return player; } }

    public T Card { get { return card; } }

    public int HandIndex { get { return handIndex; } }
}