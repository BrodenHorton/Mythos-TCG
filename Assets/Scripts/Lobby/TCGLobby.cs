using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class TcgLobby : MonoBehaviour {
    private static readonly float MAX_HEARTBEAT_TIMER_DURATION = 15f;

    private Lobby hostLobby;
    private float heartbeatTimer;

    private void Awake() {
        heartbeatTimer = 15f;
    }

    private async void Start() {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () => {
            Debug.Log("Signed in: " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private void Update() {
        LobbyHearbeat();
    }

    public async void CreateLobby() {
        try {
            hostLobby = await LobbyService.Instance.CreateLobbyAsync("My First Lobby", 4);
            TcgLogger.Log("Joined Lobby: " + hostLobby.Name + " Max players: " + hostLobby.MaxPlayers);
        }
        catch(LobbyServiceException e) {
            Debug.Log(e.Message);
        }
    }

    public async void LobbyHearbeat() {
        if (hostLobby == null)
            return;

        heartbeatTimer -= Time.deltaTime;
        if(heartbeatTimer <= 0) {
            await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
            heartbeatTimer = MAX_HEARTBEAT_TIMER_DURATION;
        }
    }

    public async void ListLobbies() {
        try {
            QueryResponse response = await LobbyService.Instance.QueryLobbiesAsync();
            TcgLogger.Log("Lobbies found " + response.Results.Count);
            foreach(Lobby lobby in response.Results)
                TcgLogger.Log(lobby.Name + " " + lobby.MaxPlayers);
        }
        catch(LobbyServiceException e) {
            Debug.Log(e.Message);
        }

    }
}
