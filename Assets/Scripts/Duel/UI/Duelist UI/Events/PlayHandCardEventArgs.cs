using System;

public class PlayHandCardEventArgs : EventArgs {
    private ulong playerId;
    private Guid handCardUuid;
    private bool isCanceled;

    public PlayHandCardEventArgs(ulong playerId, Guid handCardUuid) {
        this.playerId = playerId;
        this.handCardUuid = handCardUuid;
        isCanceled = false;
    }

    public ulong PlayerId { get { return playerId; } }

    public Guid HandCardUuid { get { return handCardUuid; } }

    public bool IsCanceled { get { return isCanceled; } set { isCanceled = value; } }
}