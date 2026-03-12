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

    private Lobby lobby;
    private bool isHost;
    private PlayerProfile playerProfile;
    private float heartbeatTimer;

    private void Awake() {
        isHost = false;
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
        if (lobby == null)
            return;

        if(isHost) {
            await LobbyService.Instance.DeleteLobbyAsync(lobby.Id);
        }
        else {
            // TODO: Add a call to leave a the joined lobby
        }
    }

    private void Update() {
        LobbyHearbeat();
    }

    public async void CreateLobby() {
        try {
            isHost = true;
            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions {
                IsPrivate = false,
                Player = GetPlayer()
            };
            lobby = await LobbyService.Instance.CreateLobbyAsync("My First Lobby", 4, createLobbyOptions);
            LobbyEventCallbacks callbacks = new LobbyEventCallbacks();
            callbacks.PlayerJoined += LobbyPlayerJoined;
            callbacks.PlayerLeft += LobbyPlayerLeft;
            callbacks.DataChanged += LobbyDataUpdated;
            callbacks.PlayerDataAdded += LobbyPlayerDataUpdated;
            callbacks.PlayerDataChanged += LobbyPlayerDataUpdated;
            await LobbyService.Instance.SubscribeToLobbyEventsAsync(lobby.Id, callbacks);
            OnLobbyCreated?.Invoke(this, new LobbyEventArgs(lobby));
            TcgLogger.Log("Created Lobby: " + lobby.Name + " Max players: " + lobby.MaxPlayers + " Lobby Code: " + lobby.LobbyCode);
            PrintPlayers(lobby);
        }
        catch(LobbyServiceException e) {
            Debug.Log(e.Message);
        }
    }

    public async void JoinLobby(string lobbyCode) {
        try {
            isHost = false;
            JoinLobbyByCodeOptions joinLobbyOptions = new JoinLobbyByCodeOptions() {
                Player = GetPlayer()
            };
            lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyOptions);
            LobbyEventCallbacks callbacks = new LobbyEventCallbacks();
            callbacks.PlayerJoined += LobbyPlayerJoined;
            callbacks.PlayerLeft += LobbyPlayerLeft;
            callbacks.DataChanged += LobbyDataUpdated;
            callbacks.PlayerDataAdded += LobbyPlayerDataUpdated;
            callbacks.PlayerDataChanged += LobbyPlayerDataUpdated;
            callbacks.LobbyDeleted += LobbyDeleted;
            callbacks.KickedFromLobby += KickedFromLobby;
            await LobbyService.Instance.SubscribeToLobbyEventsAsync(lobby.Id, callbacks);
            OnLobbyJoined?.Invoke(this, new LobbyEventArgs(lobby));
            TcgLogger.Log("You have joined a lobby");
            PrintPlayers(lobby);
        }
        catch(LobbyServiceException e) {
            Debug.Log(e.Message);
        }
    }

    public async void LobbyHearbeat() {
        if (lobby == null || !isHost)
            return;

        heartbeatTimer -= Time.deltaTime;
        if(heartbeatTimer <= 0) {
            TcgLogger.Log("Heatbeat sent");
            heartbeatTimer = MAX_HEARTBEAT_TIMER_DURATION;
            await LobbyService.Instance.SendHeartbeatPingAsync(lobby.Id);
        }
    }

    public async void UpdatePlayerReadyState(bool isReady) {
        try {
            await LobbyService.Instance.UpdatePlayerAsync(lobby.Id, AuthenticationService.Instance.PlayerId, new UpdatePlayerOptions {
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
            foreach(Lobby lobbyEntry in response.Results)
                TcgLogger.Log(lobbyEntry.Name + " " + lobbyEntry.MaxPlayers);
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

    private async void LobbyPlayerJoined(List<LobbyPlayerJoined> joinedPlayers) {
        TcgLogger.Log("Player Joined");
        lobby = await LobbyService.Instance.GetLobbyAsync(lobby.Id);
        OnPlayerJoin?.Invoke(this, new LobbyPlayersJoinedEventArgs(joinedPlayers));
    }

    private async void LobbyPlayerLeft(List<int> leftPlayerIds) {
        TcgLogger.Log("Player left");
        lobby = await LobbyService.Instance.GetLobbyAsync(lobby.Id);
        OnPlayerLeave?.Invoke(this, new LobbyPlayersLeftEventArgs(leftPlayerIds));
    }

    private async void LobbyDataUpdated(Dictionary<string, ChangedOrRemovedLobbyValue<DataObject>> lobbyChanges) {
        TcgLogger.Log("Lobby Data Updated");
        lobby = await LobbyService.Instance.GetLobbyAsync(lobby.Id);
        OnLobbyDataUpdated?.Invoke(this, new LobbyDataUpdatedEventArgs(lobbyChanges));
    }

    private async void LobbyPlayerDataUpdated(Dictionary<int, Dictionary<string, ChangedOrRemovedLobbyValue<PlayerDataObject>>> playerChanges) {
        TcgLogger.Log("Player Data Updated");
        lobby = await LobbyService.Instance.GetLobbyAsync(lobby.Id);
        OnPlayerDataUpdated?.Invoke(this, new LobbyPlayerDataUpdatedEventArgs(playerChanges));
    }

    private void LobbyDeleted() {
        TcgLogger.Log("Lobby deleted");
        lobby = null;
        OnLobbyDeleted?.Invoke(this, EventArgs.Empty);
    }

    private void KickedFromLobby() {
        OnKicked?.Invoke(this, EventArgs.Empty);
    }

    public Lobby Lobby { get {  return lobby; } }
}
