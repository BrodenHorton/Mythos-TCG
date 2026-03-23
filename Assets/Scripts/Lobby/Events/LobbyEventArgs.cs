using System;
using Unity.Services.Lobbies.Models;

public class LobbyEventArgs : EventArgs {
    private Lobby lobby;

    public LobbyEventArgs(Lobby lobby) {
        this.lobby = lobby;
    }

    public Lobby Lobby { get { return lobby; } }
}