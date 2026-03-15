using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class TcgLobby : MonoBehaviour, TcgLogSender {
    public static readonly string START_GAME_KEY = "startGame";
    public static readonly string PLAYER_NAME_KEY = "playerName";
    public static readonly string READY_STATUS_KEY = "isReady";
    private static readonly float MAX_HEARTBEAT_TIMER_DURATION = 15f;

    public event EventHandler<LobbyEventArgs> OnLobbyCreated;
    public event EventHandler<LobbyEventArgs> OnLobbyJoined;
    public event EventHandler<LobbyPlayersJoinedEventArgs> OnPlayerJoin;
    public event EventHandler<LobbyPlayersLeftEventArgs> OnPlayerLeave;
    public event EventHandler<LobbyDataUpdatedEventArgs> OnLobbyDataUpdated;
    public event EventHandler<LobbyPlayerDataUpdatedEventArgs> OnPlayerDataUpdated;
    public event EventHandler<bool> OnPlayersReadyStatusUpdated;
    public event EventHandler OnLobbyDeleted;
    public event EventHandler OnKicked;

    public static TcgLobby Instance { get; private set; }

    [SerializeField] private TcgRelay relay;

    private Lobby lobby;
    private PlayerProfile playerProfile;
    private float heartbeatTimer;

    private void Awake() {
        if (Instance != null) {
            Debug.LogWarning("TcgLobby already exists in scene. Destroying redundant object.");
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

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

        if(IsLobbyHost()) {
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
            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions {
                IsPrivate = false,
                Player = GetPlayer(),
                Data = new Dictionary<string, DataObject> {
                    { START_GAME_KEY, new DataObject(DataObject.VisibilityOptions.Member, "0") }
                }
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
            TcgLogger.Log(this, "Created Lobby: " + lobby.Name + " Max players: " + lobby.MaxPlayers + " Lobby Code: " + lobby.LobbyCode);
            PrintPlayers(lobby);
        }
        catch(LobbyServiceException e) {
            Debug.Log(e.Message);
        }
    }

    public async void JoinLobbyById(string lobbyId) {
        try {
            JoinLobbyByIdOptions joinLobbyOptions = new JoinLobbyByIdOptions() {
                Player = GetPlayer()
            };
            lobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId, joinLobbyOptions);
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
            TcgLogger.Log(this, "You have joined a lobby");
            PrintPlayers(lobby);
        }
        catch (LobbyServiceException e) {
            Debug.Log(e.Message);
        }
    }

    public async void JoinLobbyByCode(string lobbyCode) {
        try {
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
            TcgLogger.Log(this, "You have joined a lobby");
            PrintPlayers(lobby);
        }
        catch(LobbyServiceException e) {
            Debug.Log(e.Message);
        }
    }

    public async void StartGame() {
        if (!IsLobbyHost())
            throw new Exception("Attempting to start game from lobby when player is not the host");

        try {
            TcgLogger.Log(this, "Starting game");
            string relayCode = await relay.CreateRelay();
            lobby = await LobbyService.Instance.UpdateLobbyAsync(lobby.Id, new UpdateLobbyOptions {
                Data = new Dictionary<string, DataObject> {
                    { START_GAME_KEY, new DataObject(DataObject.VisibilityOptions.Member, relayCode) }
                }
            });
        }
        catch(LobbyServiceException e) {
            Debug.Log(e.Message);
        }
    }

    public async void LobbyHearbeat() {
        if (lobby == null || !IsLobbyHost())
            return;

        heartbeatTimer -= Time.deltaTime;
        if(heartbeatTimer <= 0) {
            TcgLogger.Log(this, "Heatbeat sent");
            heartbeatTimer = MAX_HEARTBEAT_TIMER_DURATION;
            await LobbyService.Instance.SendHeartbeatPingAsync(lobby.Id);
        }
    }

    public async void UpdatePlayerReadyState(bool isReady) {
        try {
            await LobbyService.Instance.UpdatePlayerAsync(lobby.Id, AuthenticationService.Instance.PlayerId, new UpdatePlayerOptions {
                Data = new Dictionary<string, PlayerDataObject> {
                    { READY_STATUS_KEY, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, isReady.ToString()) }
                }
            });
        }
        catch (LobbyServiceException e) {
            Debug.Log(e.Message);
        }
    }

    public async Task<List<Lobby>> GetLobbyList() {
        QueryResponse response = null;
        try {
            QueryLobbiesOptions lobbyOptions = new QueryLobbiesOptions();
            lobbyOptions.Filters = new List<QueryFilter> {
                new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
            };
            lobbyOptions.Order = new List<QueryOrder> {
                new QueryOrder(false, QueryOrder.FieldOptions.Created)
            };
            response = await LobbyService.Instance.QueryLobbiesAsync();
            Debug.Log("Lobbies found " + response.Results.Count);
        }
        catch(LobbyServiceException e) {
            Debug.Log(e.Message);
        }

        return response != null ? response.Results : new List<Lobby>();
    }

    private Player GetPlayer() {
        return new Player {
            Data = new Dictionary<string, PlayerDataObject> {
                { PLAYER_NAME_KEY, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, playerProfile.Username) },
                { READY_STATUS_KEY, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, "false") }
            }
        };
    }

    public bool IsLobbyHost() {
        if(lobby == null)
            throw new Exception("Attempting to check lobby host when lobby is null");

        return AuthenticationService.Instance.PlayerId == lobby.HostId;
    }

    public async void ListLobbies() {
        List<Lobby> lobbies = await GetLobbyList();
        if (lobbies != null) {
            foreach (Lobby lobbyEntry in lobbies)
                TcgLogger.Log(lobbyEntry.Name + " " + lobbyEntry.MaxPlayers);
        }
    }

    private void PrintPlayers(Lobby lobby) {
        TcgLogger.Log(this, "Players in lobby:");
        foreach(Player player in lobby.Players)
            TcgLogger.Log(this, player.Id + " " + player.Data[PLAYER_NAME_KEY].Value);
    }

    private bool AreAllPlayersReady() {
        foreach(Player player in lobby.Players) {
            if (!bool.Parse(player.Data[READY_STATUS_KEY].Value))
                return false;
        }

        return true;
    }

    private async void LobbyPlayerJoined(List<LobbyPlayerJoined> joinedPlayers) {
        TcgLogger.Log(this, "Player Joined");
        lobby = await LobbyService.Instance.GetLobbyAsync(lobby.Id);
        OnPlayerJoin?.Invoke(this, new LobbyPlayersJoinedEventArgs(joinedPlayers));
    }

    private async void LobbyPlayerLeft(List<int> leftPlayerIds) {
        TcgLogger.Log(this, "Player left");
        lobby = await LobbyService.Instance.GetLobbyAsync(lobby.Id);
        OnPlayerLeave?.Invoke(this, new LobbyPlayersLeftEventArgs(leftPlayerIds));
    }

    private async void LobbyDataUpdated(Dictionary<string, ChangedOrRemovedLobbyValue<DataObject>> lobbyChanges) {
        TcgLogger.Log(this, "Lobby Data Updated");
        lobby = await LobbyService.Instance.GetLobbyAsync(lobby.Id);
        OnLobbyDataUpdated?.Invoke(this, new LobbyDataUpdatedEventArgs(lobbyChanges));
        if (!IsLobbyHost() && lobby.Data[START_GAME_KEY].Value != "0") {
            relay.JoinRelay(lobby.Data[START_GAME_KEY].Value);
            TcgMultiplayerManager.Instance.StartClient();
        }
    }

    private async void LobbyPlayerDataUpdated(Dictionary<int, Dictionary<string, ChangedOrRemovedLobbyValue<PlayerDataObject>>> playerChanges) {
        TcgLogger.Log(this, "Player Data Updated");
        lobby = await LobbyService.Instance.GetLobbyAsync(lobby.Id);
        OnPlayerDataUpdated?.Invoke(this, new LobbyPlayerDataUpdatedEventArgs(playerChanges));
        if (IsLobbyHost()) {
            OnPlayersReadyStatusUpdated?.Invoke(this, AreAllPlayersReady());
        }
    }

    private void LobbyDeleted() {
        TcgLogger.Log(this, "Lobby deleted");
        lobby = null;
        OnLobbyDeleted?.Invoke(this, EventArgs.Empty);
    }

    private void KickedFromLobby() {
        OnKicked?.Invoke(this, EventArgs.Empty);
    }

    public string GetLogPrefix() {
        return "[&bLobby&f]";
    }

    public Lobby Lobby { get {  return lobby; } }
}
