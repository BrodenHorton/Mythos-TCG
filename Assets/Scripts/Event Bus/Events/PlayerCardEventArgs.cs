using System;

public class PlayerCardEventArgs<T> : EventArgs where T : Card {
    private ulong playerId;
    private T card;

    public PlayerCardEventArgs(ulong playerId, T card) {
        this.playerId = playerId;
        this.card = card;
    }

    public ulong PlayerId { get { return playerId; } }

    public T Card { get { return card; } }
}
