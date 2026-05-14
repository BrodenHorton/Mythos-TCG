using System;

public class CardRemovedFromHandEventArgs : EventArgs {
    private ulong playerId;
    private Card card;

    public CardRemovedFromHandEventArgs(ulong playerId, Card card) {
        this.playerId = playerId;
        this.card = card;
    }

    public ulong PlayerId { get { return playerId; } }

    public Card Card { get { return card; } }
}