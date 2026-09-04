using System;

public class PlayerEventArgs : EventArgs {
    private ulong playerId;

    public PlayerEventArgs(ulong playerId) {
        this.playerId = playerId;
    }

    public ulong PlayerId { get { return playerId; } }
}
