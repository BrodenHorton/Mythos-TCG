using System;

public class PlayerCardEventArgs<T> : EventArgs where T : Card {
    private MatchPlayer player;
    private T card;

    public PlayerCardEventArgs(MatchPlayer player, T card) {
        this.player = player;
        this.card = card;
    }

    public MatchPlayer Player { get { return player; } }

    public T Card { get { return card; } }
}
