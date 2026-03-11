using System;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class TcgLobby : MonoBehaviour {
    private static readonly float MAX_HEARTBEAT_TIMER_DURATION = 15f;

    public event EventHandler<LobbyEventArgs> OnLobbyCreated;
    public event EventHandler<LobbyEventArgs> OnLobbyJoined;
    public event EventHandler<LobbyEventArgs> OnLobbyUpdated;

    private Lobby hostLobby;
    private ILobbyEvents hsotLobbyEvents;
    private Lobby joinedLobby;
    private PlayerProfile playerProfile;
    private float heartbeatTimer;
    private float lobbyPollUpdatesTimer;

    private void Awake() {
        heartbeatTimer = 15f;
        lobbyPollUpdatesTimer = 1.1f;
        playerProfile = FindFirstObjectByType<PlayerProfile>();

        // TODO: Move to the Create Lobby method
        LobbyEventCallbacks callbacks = new LobbyEventCallbacks();
        callbacks.LobbyChanged += InvokeLobbyUpdate;
        hsotLobbyEvents = LobbyService.Instance.SubscribeToLobbyEventsAsync(hostLobby.Id, callbacks).Result;
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
        PollLobbyUpdates();
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
            joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyOptions);
            TcgLogger.Log("Joined lobby with code: " + joinedLobby);
            PrintPlayers(joinedLobby);
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

    public async void PollLobbyUpdates() {
        if (joinedLobby == null)
            return;

        lobbyPollUpdatesTimer -= Time.deltaTime;
        if(lobbyPollUpdatesTimer <= 0) {
            lobbyPollUpdatesTimer = 1.1f;
            joinedLobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
        }
    }

    public async void UpdatePlayerReadyState(bool isReady) {
        try {
            await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId, new UpdatePlayerOptions {
                Data = new Dictionary<string, PlayerDataObject> {
                    { "isReady", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, isReady.ToString()) }
                }
            });
        }
        catch (LobbyServiceException e) {
            Debug.Log(e.Message);
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
                { "playerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerProfile.Username) }
            }
        };
    }

    private void PrintPlayers(Lobby lobby) {
        TcgLogger.Log("Players in lobby:");
        foreach(Player player in lobby.Players)
            TcgLogger.Log(player.Id + " " + player.Data["playerName"].Value);
    }

    private void InvokeLobbyUpdate(ILobbyChanges lobbyChanges) {
        OnLobbyUpdated?.Invoke(this, new LobbyEventArgs(hostLobby));
    }
}
