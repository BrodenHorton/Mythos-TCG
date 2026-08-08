using System;

public class ManaChangedEventArgs : EventArgs {
    private ulong playerId;
    private int manaCount;

    public ManaChangedEventArgs(ulong playerId, int manaCount) {
        this.playerId = playerId;
        this.manaCount = manaCount;
    }

    public ulong PlayerId { get { return playerId; } }

    public int ManaCount { get { return manaCount; } }
}
