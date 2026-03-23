using System;
using System.Collections.Generic;

public class LobbyPlayersLeftEventArgs : EventArgs {
    private List<int> leftPlayerIds;

    public LobbyPlayersLeftEventArgs(List<int> leftPlayerIds) {
        this.leftPlayerIds = leftPlayerIds;
    }

    public List<int> LeftPlayerIds { get { return leftPlayerIds; } }
}
