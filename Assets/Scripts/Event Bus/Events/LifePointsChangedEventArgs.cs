using System;

public class LifePointsChangedEventArgs : EventArgs {
    private ulong playerId;
    private int previousLifePoints;
    private int lifePoints;

    public LifePointsChangedEventArgs(ulong playerId, int previousLifePoints, int lifePoints) {
        this.playerId = playerId;
        this.previousLifePoints = previousLifePoints;
        this.lifePoints = lifePoints;
    }

    public ulong PlayerId { get { return playerId; } }

    public int PreviousLifePoints { get { return previousLifePoints; } }

    public int LifePoints { get { return lifePoints; } }
}