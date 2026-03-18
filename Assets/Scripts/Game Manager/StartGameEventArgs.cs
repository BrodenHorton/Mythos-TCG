using System;
using System.Collections.Generic;

public class StartGameEventArgs : EventArgs {
    private List<ulong> playerOrder;

    public StartGameEventArgs(List<ulong> playerOrder) {
        this.playerOrder = playerOrder;
    }

    public List<ulong> PlayerOrder { get { return playerOrder; } }
}