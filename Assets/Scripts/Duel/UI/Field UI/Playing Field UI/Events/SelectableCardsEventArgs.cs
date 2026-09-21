using System;
using System.Collections.Generic;

public class SelectableCardsEventArgs : PlayerEventArgs {
    private List<Guid> cardUuids;

    public SelectableCardsEventArgs(ulong playerId, List<Guid> cardUuids) : base(playerId) {
        this.cardUuids = cardUuids;
    }

    public List<Guid> CardUuids { get { return cardUuids; } }
}
