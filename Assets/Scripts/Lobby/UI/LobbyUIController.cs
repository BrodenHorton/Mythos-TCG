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
        //tcgLobby.OnPlayerDataUpdated += 
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
        TcgLogger.Log(player.Data["playerName"].Value + " has joined the lobby!");
        lobbyUI.AddLobbyPlayer(player.Id, player.Data["playerName"].Value, false);
    }

    public void RemoveLobbyPlayer(string playerId) {
        lobbyUI.RemoveLobbyPlayer(playerId);
    }

    public void UpdatePlayersReadyStatus() {

    }
}