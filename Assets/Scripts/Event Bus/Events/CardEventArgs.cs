using System;

public class CardEventArgs<T> : EventArgs where T : Card {
    private T card;

    public CardEventArgs(T card) {
        this.card = card;
    }

    public T Card { get { return card; } }
}
