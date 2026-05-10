using System;

public class PlayCardFromHandEventArgs<T> : EventArgs where T : Card {
    private ulong playerId;
    private T card;

    public PlayCardFromHandEventArgs(ulong playerId, T card) {
        this.playerId = playerId;
        this.card = card;
    }

    public ulong PlayerId { get { return playerId; } }

    public T Card { get { return card; } }
}