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
    public event EventHandler<LobbyPlayersJoinedEventArgs> OnPlayerJoin;
    public event EventHandler<LobbyPlayersLeftEventArgs> OnPlayerLeave;
    public event EventHandler<LobbyDataUpdatedEventArgs> OnLobbyDataUpdated;
    public event EventHandler<LobbyPlayerDataUpdatedEventArgs> OnPlayerDataUpdated;
    public event EventHandler OnLobbyDeleted;
    public event EventHandler OnKicked;

    private Lobby hostLobby;
    private Lobby joinedLobby;
    private PlayerProfile playerProfile;
    private float heartbeatTimer;

    private void Awake() {
        heartbeatTimer = 15f;
        playerProfile = FindFirstObjectByType<PlayerProfile>();
    }

    private async void Start() {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () => {
            TcgLogger.Log("Signed in: " + AuthenticationService.Instance.PlayerId);
        };
        AuthenticationService.Instance.SwitchProfile(UnityEngine.Random.Range(0, 1000000).ToString());
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private async void OnDestroy() {
        if(hostLobby != null) {
            await LobbyService.Instance.DeleteLobbyAsync(hostLobby.Id);
        }
        if(joinedLobby != null) {
            // TODO: Add a call to leave a the joined lobby
        }
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
            LobbyEventCallbacks callbacks = new LobbyEventCallbacks();
            callbacks.PlayerJoined += HostLobbyPlayerJoined;
            callbacks.PlayerLeft += HostLobbyPlayerLeft;
            callbacks.DataChanged += HostLobbyDataUpdated;
            callbacks.PlayerDataChanged += HostLobbyPlayerDataUpdated;
            await LobbyService.Instance.SubscribeToLobbyEventsAsync(hostLobby.Id, callbacks);
            OnLobbyCreated?.Invoke(this, new LobbyEventArgs(hostLobby));
            TcgLogger.Log("Joined Lobby: " + hostLobby.Name + " Max players: " + hostLobby.MaxPlayers + " Lobby Code: " + hostLobby.LobbyCode);
            PrintPlayers(hostLobby);
        }
        catch(LobbyServiceException e) {
            Debug.Log(e.Message);
        }
    }

    public async void JoinLobby(string lobbyCode) {
        TcgLogger.Log("Join 1");
        try {
            JoinLobbyByCodeOptions joinLobbyOptions = new JoinLobbyByCodeOptions() {
                Player = GetPlayer()
            };
            TcgLogger.Log("Join 2");
            joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyOptions);
            TcgLogger.Log("Join 3");
            LobbyEventCallbacks callbacks = new LobbyEventCallbacks();
            callbacks.PlayerJoined += JoinedLobbyPlayerJoined;
            callbacks.PlayerLeft += JoinedLobbyPlayerLeft;
            callbacks.DataChanged += JoinedLobbyDataUpdated;
            callbacks.PlayerDataChanged += JoinedLobbyPlayerDataUpdated;
            callbacks.LobbyDeleted += LobbyDeleted;
            callbacks.KickedFromLobby += KickedFromLobby;
            TcgLogger.Log("Join 4");
            await LobbyService.Instance.SubscribeToLobbyEventsAsync(joinedLobby.Id, callbacks);
            TcgLogger.Log("Join 5");
            OnLobbyJoined?.Invoke(this, new LobbyEventArgs(joinedLobby));
            TcgLogger.Log("You have joined a lobby");
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

    private void HostLobbyPlayerJoined(List<LobbyPlayerJoined> joinedPlayers) {
        TcgLogger.Log("Host lobby Player Joined");
        OnPlayerJoin?.Invoke(this, new LobbyPlayersJoinedEventArgs(joinedPlayers));
    }

    private async void JoinedLobbyPlayerJoined(List<LobbyPlayerJoined> joinedPlayers) {
        TcgLogger.Log("Joined lobby Player Joined");
        joinedLobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
        OnPlayerJoin?.Invoke(this, new LobbyPlayersJoinedEventArgs(joinedPlayers));
    }

    private void HostLobbyPlayerLeft(List<int> leftPlayerIds) {
        TcgLogger.Log("Host lobby Player Left");
        OnPlayerLeave?.Invoke(this, new LobbyPlayersLeftEventArgs(leftPlayerIds));
    }

    private async void JoinedLobbyPlayerLeft(List<int> leftPlayerIds) {
        TcgLogger.Log("Joined lobby Player left");
        joinedLobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
        OnPlayerLeave?.Invoke(this, new LobbyPlayersLeftEventArgs(leftPlayerIds));
    }

    private void HostLobbyDataUpdated(Dictionary<string, ChangedOrRemovedLobbyValue<DataObject>> lobbyChanges) {
        TcgLogger.Log("Host lobby Lobby Data Updated");
        OnLobbyDataUpdated?.Invoke(this, new LobbyDataUpdatedEventArgs(lobbyChanges));
    }

    private async void JoinedLobbyDataUpdated(Dictionary<string, ChangedOrRemovedLobbyValue<DataObject>> lobbyChanges) {
        TcgLogger.Log("Joined lobby Lobby Data Updated");
        joinedLobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
        OnLobbyDataUpdated?.Invoke(this, new LobbyDataUpdatedEventArgs(lobbyChanges));
    }

    private void HostLobbyPlayerDataUpdated(Dictionary<int, Dictionary<string, ChangedOrRemovedLobbyValue<PlayerDataObject>>> playerChanges) {
        TcgLogger.Log("Host lobby Player Data Updated");
        OnPlayerDataUpdated?.Invoke(this, new LobbyPlayerDataUpdatedEventArgs(playerChanges));
    }

    private async void JoinedLobbyPlayerDataUpdated(Dictionary<int, Dictionary<string, ChangedOrRemovedLobbyValue<PlayerDataObject>>> playerChanges) {
        TcgLogger.Log("Joined lobby Player Data Updated");
        joinedLobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
        OnPlayerDataUpdated?.Invoke(this, new LobbyPlayerDataUpdatedEventArgs(playerChanges));
    }

    private void LobbyDeleted() {
        TcgLogger.Log("Lobby deleted");
        joinedLobby = null;
        OnLobbyDeleted?.Invoke(this, EventArgs.Empty);
    }

    private void KickedFromLobby() {
        OnKicked?.Invoke(this, EventArgs.Empty);
    }
}
