using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DuelManager : NetworkBehaviour {
    private static readonly int STARTING_LIFE_POINTS = 20;
    private static readonly int STARTING_MANA_COUNT = 0;
    public static readonly int INITIAL_HAND_SIZE = 5;

    public event EventHandler<PlayersInitializedEventArgs> OnPlayersInitialization;
    public event EventHandler OnPlayersInitializationFinished;
    public event EventHandler<NextPlayerTurnEventArgs> OnNextPlayerTurn;
    public event EventHandler<int> OnNextPlayerTurnClient;
    public event EventHandler<int> OnNextFullTurn;

    [SerializeField] private DeckSimulation deckSim;

    private List<MatchPlayer> players;
    private int currentPlayerTurnIndex;
    private int fullTurnCount;

    private void Awake() {
        currentPlayerTurnIndex = 0;
        fullTurnCount = 1;

        ServiceLocator.Register(this);
    }

    private void Start() {
        if (!IsServer)
            return;

        GameManager.Instance.OnGameStart += InitializePlayers;
    }

    public override void OnNetworkDespawn() {
        ServiceLocator.Unregister(this);
    }

    private void InitializePlayers(object sender, StartGameEventArgs args) {
        if (!IsServer)
            return;

        InitializePlayers(args.PlayerOrder.ToArray());
    }

    private void InitializePlayers(ulong[] playerOrder) {
        if (!IsServer)
            return;

        players = new List<MatchPlayer>();
        for (int i = 0; i < playerOrder.Length; i++) {
            List<Card> deck = deckSim.GenerateDeck(playerOrder[i]);
            MatchPlayer player = new MatchPlayer(playerOrder[i], deck);
            players.Add(player);
        }

        for(int i = 0; i < playerOrder.Length; i++) {
            BaseRpcTarget target = RpcTarget.Group(new List<ulong>() { playerOrder[i] }, RpcTargetUse.Temp);
            InvokePlayerInitializationClientRpc(playerOrder, i, target);
        }
        OnPlayersInitializationFinished?.Invoke(this, EventArgs.Empty);
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void InvokePlayerInitializationClientRpc(ulong[] playerOrder, int localClientPlayerIndex, RpcParams rpcParams) {
        OnPlayersInitialization?.Invoke(this, new PlayersInitializedEventArgs(new List<ulong>(playerOrder), localClientPlayerIndex, STARTING_LIFE_POINTS, STARTING_MANA_COUNT));
    }

    public void NextTurn() {
        if (!IsServer)
            return;

        RegenerateCreaturesHealth();
        currentPlayerTurnIndex = ++currentPlayerTurnIndex % players.Count;
        InvokeOnNextPlayerTurnClientClientRpc(currentPlayerTurnIndex);
        OnNextPlayerTurn?.Invoke(this, new NextPlayerTurnEventArgs(GetCurrentPlayerTurn(), currentPlayerTurnIndex));
        if (currentPlayerTurnIndex == 0) {
            fullTurnCount++;
            InvokeOnNextFullTurnClientRpc(fullTurnCount);
        }
    }

    private void RegenerateCreaturesHealth() {
        if (!IsServer)
            return;

        List<CreatureCardPayload> regeneratedCreatureCardPayloads = new List<CreatureCardPayload>();
        foreach (MatchPlayer player in Players) {
            for (int i = 0; i < player.Creatures.Count; i++) {
                if (player.Creatures[i].CurrentDamage > 0) {
                    PlayerCardCancelableEventArgs<CreatureCard> args = new PlayerCardCancelableEventArgs<CreatureCard>(player.PlayerId, player.Creatures[i]);
                    EventBus.Instance.InvokeOnCreatureEndOfTurnRegeneration(args);
                    if(!args.IsCanceled) {
                        player.Creatures[i].CurrentDamage = 0;
                        regeneratedCreatureCardPayloads.Add(new CreatureCardPayload(player.Creatures[i]));
                    }
                }
            }
        }
        EventBus.Instance.InvokeOnCreatureEndOfTurnRegenerationFinishedClientRpc(regeneratedCreatureCardPayloads.ToArray());
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void InvokeOnNextPlayerTurnClientClientRpc(int playerIndex) {
        OnNextPlayerTurnClient?.Invoke(this, playerIndex);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void InvokeOnNextFullTurnClientRpc(int fullTurnCount) {
        OnNextFullTurn?.Invoke(this, fullTurnCount);
    }

    public int GetPlayerCount() {
        return players.Count;
    }

    public MatchPlayer GetCurrentPlayerTurn() {
        return players[currentPlayerTurnIndex];
    }

    public int GetStartOfTurnManaCount() {
        return fullTurnCount;
    }

    public void IncrementFullTurnCount() {
        fullTurnCount++;
    }

    public MatchPlayer GetPlayerById(ulong playerId) {
        foreach (MatchPlayer p in players) {
            if (p.PlayerId == playerId)
                return p;
        }

        throw new Exception("Unable to find player with playerId: " + playerId);
    }

    public int GetPlayerIndex(MatchPlayer player) {
        return GetPlayerIndex(player.PlayerId);
    }

    public int GetPlayerIndex(ulong playerId) {
        for(int i = 0; i < players.Count; i++) {
            if (players[i].PlayerId == playerId)
                return i;
        }

        throw new Exception("Player index could not be found for playerId: " + playerId);
    }

    public List<ulong> GetPlayerIds() {
        List<ulong> playerIds = new List<ulong>();
        foreach(MatchPlayer player in players) {
            playerIds.Add(player.PlayerId);
        }

        return playerIds;
    }

    public List<MatchPlayer> Players { get { return players; } }

    public int CurrentPlayerTurnIndex {  get { return currentPlayerTurnIndex; } }

    public int FullTurnCount { get { return fullTurnCount; } }
}
