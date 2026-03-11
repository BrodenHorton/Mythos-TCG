using System;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;

public class LobbyPlayerDataUpdatedEventArgs : EventArgs {
    private Dictionary<int, Dictionary<string, ChangedOrRemovedLobbyValue<PlayerDataObject>>> playerChanges;

    public LobbyPlayerDataUpdatedEventArgs(Dictionary<int, Dictionary<string, ChangedOrRemovedLobbyValue<PlayerDataObject>>> playerChanges) {
        this.playerChanges = playerChanges;
    }

    public Dictionary<int, Dictionary<string, ChangedOrRemovedLobbyValue<PlayerDataObject>>> PlayerChanges { get { return playerChanges; } }
}