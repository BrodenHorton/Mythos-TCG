using System;

public class LifePointsChangedEventArgs : EventArgs {
    private MatchPlayer player;
    private int lifePoints;

    public LifePointsChangedEventArgs(MatchPlayer player, int lifePoints) {
        this.player = player;
        this.lifePoints = lifePoints;
    }

    public MatchPlayer Player { get { return player; } }

    public int LifePoints { get { return lifePoints; } }
}