using System;
using System.Collections.Generic;
using Unity.Services.Lobbies;

public class LobbyPlayersJoinedEventArgs : EventArgs {
    private List<LobbyPlayerJoined> joinedPlayers;

    public LobbyPlayersJoinedEventArgs(List<LobbyPlayerJoined> joinedPlayers) {
        this.joinedPlayers = joinedPlayers;
    }

    public List<LobbyPlayerJoined> JoinedPlayers { get { return joinedPlayers; } }
}
