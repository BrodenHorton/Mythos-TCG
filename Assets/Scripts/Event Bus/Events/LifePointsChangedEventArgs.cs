using System;

public class LifePointsChangedEventArgs : EventArgs {
    private ulong playerId;
    private int lifePoints;

    public LifePointsChangedEventArgs(ulong playerId, int lifePoints) {
        this.playerId = playerId;
        this.lifePoints = lifePoints;
    }

    public ulong PlayerId { get { return playerId; } }

    public int LifePoints { get { return lifePoints; } }
}