using System;

public class ManaChangedEventArgs : EventArgs {
    private ulong playerId;
    private int currentMana;

    public ManaChangedEventArgs(ulong playerId, int currentMana) {
        this.playerId = playerId;
        this.currentMana = currentMana;
    }

    public ulong PlayerId { get { return playerId; } }

    public int CurrentMana { get { return currentMana; } }
}
