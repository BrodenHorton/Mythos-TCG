using System;

public class PlayerFieldCardEventArgs<T> : EventArgs where T : FieldCardUI {
    private ulong playerId;
    private T card;

    public PlayerFieldCardEventArgs(ulong playerId, T card) {
        this.playerId = playerId;
        this.card = card;
    }

    public ulong PlayerId { get { return playerId; } }

    public T Card { get { return card; } }
}