using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class TcgLobby : MonoBehaviour {
    private static readonly float MAX_HEARTBEAT_TIMER_DURATION = 15f;

    private Lobby hostLobby;
    private float heartbeatTimer;

    private string playerName;

    private void Awake() {
        heartbeatTimer = 15f;
        playerName = "Omnibit" + UnityEngine.Random.Range(0, 100);
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
            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions {
                IsPrivate = false,
                Player = GetPlayer()
            };
            hostLobby = await LobbyService.Instance.CreateLobbyAsync("My First Lobby", 4, createLobbyOptions);
            TcgLogger.Log("Joined Lobby: " + hostLobby.Name + " Max players: " + hostLobby.MaxPlayers + " Lobby Code: " + hostLobby.LobbyCode);
            PrintPlayers(hostLobby);
        }
        catch(LobbyServiceException e) {
            Debug.Log(e.Message);
        }
    }

    public async void JoinLobby(string lobbyCode) {
        try {
            JoinLobbyByCodeOptions joinLobbyOptions = new JoinLobbyByCodeOptions() {
                Player = GetPlayer()
            };
            Lobby lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyOptions);
            TcgLogger.Log("Joined lobby with code: " + lobbyCode);
            PrintPlayers(lobby);
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
            TcgLogger.Log("Heatbeat sent");
            heartbeatTimer = MAX_HEARTBEAT_TIMER_DURATION;
            await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
        }
    }

    public async void ListLobbies() {
        try {
            QueryLobbiesOptions lobbyOptions = new QueryLobbiesOptions();
            lobbyOptions.Filters = new List<QueryFilter> {
                new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
            };
            lobbyOptions.Order = new List<QueryOrder> {
                new QueryOrder(false, QueryOrder.FieldOptions.Created)
            };
            QueryResponse response = await LobbyService.Instance.QueryLobbiesAsync();
            TcgLogger.Log("Lobbies found " + response.Results.Count);
            foreach(Lobby lobby in response.Results)
                TcgLogger.Log(lobby.Name + " " + lobby.MaxPlayers);
        }
        catch(LobbyServiceException e) {
            Debug.Log(e.Message);
        }
    }

    private Player GetPlayer() {
        return new Player {
            Data = new Dictionary<string, PlayerDataObject> {
                { "playerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) }
            }
        };
    }

    private void PrintPlayers(Lobby lobby) {
        TcgLogger.Log("Players in lobby:");
        foreach(Player player in lobby.Players)
            TcgLogger.Log(player.Id + " " + player.Data["playerName"].Value);
    }
}
