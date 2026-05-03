using System;
using System.Collections.Generic;

public class PlayersInitializedEventArgs : EventArgs {
    private List<ulong> playerOrder;
    private int localClientPlayerIndex;
    private int initialLifePoints;
    private int initialManaCount;

    public PlayersInitializedEventArgs(List<ulong> playerOrder, int localClientPlayerIndex, int initialLifePoints, int initialManaCount) {
        this.playerOrder = playerOrder;
        this.localClientPlayerIndex = localClientPlayerIndex;
        this.initialLifePoints = initialLifePoints;
        this.initialManaCount = initialManaCount;
    }

    public List<ulong> PlayerOrder { get { return playerOrder; } }

    public int LocalClientPlayerIndex { get { return localClientPlayerIndex; } }

    public int InitialLifePoints { get { return initialLifePoints; } }

    public int InitialManaCount { get { return initialManaCount; } }
}