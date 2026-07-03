using System;
using System.Collections.Generic;

public class SelectableCardsEventArgs : EventArgs {
    private ulong playerId;
    private List<Guid> cardUuids;

    public SelectableCardsEventArgs(ulong playerId, List<Guid> cardUuids) {
        this.playerId = playerId;
        this.cardUuids = cardUuids;
    }

    public ulong PlayerId { get { return playerId; } }

    public List<Guid> CardUuids { get { return cardUuids; } }
}
