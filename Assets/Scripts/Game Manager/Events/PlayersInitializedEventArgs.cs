using System;

public class PlayersInitializedEventArgs : EventArgs {
    private int playerCount;
    private int localClientPlayerIndex;

    public PlayersInitializedEventArgs(int playerCount, int localClientPlayerIndex) {
        this.playerCount = playerCount;
        this.localClientPlayerIndex = localClientPlayerIndex;
    }

    public int PlayerCount { get {return playerCount; } }

    public int LocalClientPlayerIndex { get {return localClientPlayerIndex; } }
}