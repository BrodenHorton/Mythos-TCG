using System;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;

public class LobbyDataUpdatedEventArgs : EventArgs {
    private Dictionary<string, ChangedOrRemovedLobbyValue<DataObject>> lobbyChanges;

    public LobbyDataUpdatedEventArgs(Dictionary<string, ChangedOrRemovedLobbyValue<DataObject>> lobbyChanges) {
        this.lobbyChanges = lobbyChanges;
    }

    public Dictionary<string, ChangedOrRemovedLobbyValue<DataObject>> LobbyChanges { get { return lobbyChanges; } }
}
