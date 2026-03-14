using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyUIController : MonoBehaviour {
    [SerializeField] TcgLobby tcgLobby;
    [SerializeField] private LobbyUI lobbyUI;

    private void Start() {
        tcgLobby.OnLobbyCreated += JoinLobby;
        tcgLobby.OnLobbyJoined += JoinLobby;
        tcgLobby.OnPlayerJoin += AddLobbyPlayers;
        //tcgLobby.OnPlayerLeave += 
        //tcgLobby.OnPlayerKicked += 
        //tcgLobby.OnLobbyDataUpdated += 
        tcgLobby.OnPlayerDataUpdated += UpdateLobbyPlayersData;
        //tcgLobby.OnLobbyDeleted += 
        //tcgLobby.OnKicked += 

        lobbyUI.ReadyBtn.onClick.AddListener(() => {
            tcgLobby.UpdatePlayerReadyState(true);
            lobbyUI.ReadyBtn.gameObject.SetActive(false);
        });
    }

    public void JoinLobby(object sender, LobbyEventArgs args) {
        lobbyUI.gameObject.SetActive(true);
        for (int i = 0; i < args.Lobby.Players.Count; i++) {
            AddLobbyPlayer(args.Lobby.Players[i]);
        }
    }

    public void AddLobbyPlayers(object sender, LobbyPlayersJoinedEventArgs args) {
        for (int i = 0; i < args.JoinedPlayers.Count; i++) {
            AddLobbyPlayer(args.JoinedPlayers[i].Player);
        }
    }

    public void AddLobbyPlayer(Player player) {
        TcgLogger.Log("&e" + player.Data["playerName"].Value + " has joined the lobby!");
        lobbyUI.AddLobbyPlayer(player.Id, player.Data["playerName"].Value, false);
    }

    public void RemoveLobbyPlayer(string playerId) {
        lobbyUI.RemoveLobbyPlayer(playerId);
    }

    public void UpdateLobbyPlayersData(object sender, LobbyPlayerDataUpdatedEventArgs args) {
        foreach (KeyValuePair<int, Dictionary<string, ChangedOrRemovedLobbyValue<PlayerDataObject>>> playerChanges in args.PlayerChanges) {
            foreach (KeyValuePair<string, ChangedOrRemovedLobbyValue<PlayerDataObject>> change in playerChanges.Value) {
                if (change.Key == "isReady" && change.Value.Changed) {
                    bool isReady = bool.Parse(change.Value.Value.Value);
                    TcgLogger.Log("isReady: " + isReady);
                    TcgLogger.Log("Player Index: " + playerChanges.Key);
                    TcgLogger.Log("Number of Lobby Players: " + tcgLobby.Lobby.Players.Count);
                    lobbyUI.UpdateLobbyPlayerData(tcgLobby.Lobby.Players[playerChanges.Key].Id, isReady);
                }
            }
        }
    }
}