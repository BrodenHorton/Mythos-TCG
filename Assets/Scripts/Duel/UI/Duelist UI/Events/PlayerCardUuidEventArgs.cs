using System;

public class PlayerCardUuidEventArgs : EventArgs {
    private ulong playerId;
    private Guid cardUuid;
    private bool isCanceled;

    public PlayerCardUuidEventArgs(ulong playerId, Guid cardUuid) {
        this.playerId = playerId;
        this.cardUuid = cardUuid;
        isCanceled = false;
    }

    public ulong PlayerId { get { return playerId; } }

    public Guid CardUuid { get { return cardUuid; } }

    public bool IsCanceled { get { return isCanceled; } set { isCanceled = value; } }
}