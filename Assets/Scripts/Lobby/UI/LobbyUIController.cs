using UnityEngine;

public class LobbyUIController : MonoBehaviour {
    [SerializeField] TcgLobby tcgLobby;
    [SerializeField] private LobbyUI lobbyUI;

    private void Start() {
        //tcgLobby.OnLobbyCreated +=
        //tcgLobby.OnLobbyJoined +=
        tcgLobby.OnPlayerJoin += AddLobbyPlayers;
        //tcgLobby.OnPlayerLeave += 
        //tcgLobby.OnPlayerKicked += 
        //tcgLobby.OnLobbyDataUpdated += 
        //tcgLobby.OnPlayerDataUpdated += 
        //tcgLobby.OnLobbyDeleted += 
        //tcgLobby.OnKicked += 
    }

    public void AddLobbyPlayers(object sender, LobbyPlayersJoinedEventArgs args) {
        for (int i = 0; i < args.JoinedPlayers.Count; i++) {
            TcgLogger.Log(args.JoinedPlayers[i].Player.Data["playerName"].Value + " has joined the lobby!");
        }
    }

    public void RemoveLobbyPlayer(string playerId) {
        lobbyUI.RemoveLobbyPlayer(playerId);
    }

    public void UpdatePlayersReadyStatus() {

    }
}