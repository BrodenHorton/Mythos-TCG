using System;

public class LifePointsChangedEventArgs : PlayerEventArgs {
    private int previousLifePoints;
    private int lifePoints;

    public LifePointsChangedEventArgs(ulong playerId, int previousLifePoints, int lifePoints) : base(playerId) {
        this.previousLifePoints = previousLifePoints;
        this.lifePoints = lifePoints;
    }

    public int PreviousLifePoints { get { return previousLifePoints; } }

    public int LifePoints { get { return lifePoints; } }
}