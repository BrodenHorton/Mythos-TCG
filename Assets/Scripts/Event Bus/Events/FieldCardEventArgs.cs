using System;

public class FieldCardEventArgs<T> : EventArgs where T : FieldCardUI {
    private T card;

    public FieldCardEventArgs(ulong playerId, T card) {
        this.card = card;
    }

    public T Card { get { return card; } }
}