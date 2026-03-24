using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// DuelManager should only be used by the server
public class DuelManager : NetworkBehaviour {
    public event EventHandler<PlayersInitializedEventArgs> OnPlayersInitialization;
    public event EventHandler<PlayersInitializedEventArgs> OnPlayersInitializationFinished;
    public event EventHandler<NextPlayerTurnEventArgs> OnNextPlayerTurn;
    public event EventHandler<NextFullTurnEventArgs> OnNextFullTurn;

    private List<MatchPlayer> players;
    private int currentPlayerTurnIndex;
    private int fullTurnCount;

    private void Awake() {
        currentPlayerTurnIndex = 0;
        fullTurnCount = 1;
    }

    private void Start() {
        if(IsServer)
            GameManager.Instance.OnGameStart += InitializePlayersServerRpc;
    }

    [Rpc(SendTo.Server)]
    public void InitializePlayersServerRpc(object sender, StartGameEventArgs args) {
        players = new List<MatchPlayer>();
        for(int i = 0; i < args.PlayerOrder.Count; i++) {
            MatchPlayer player = new MatchPlayer(args.PlayerOrder[i], Temp_PopulateDeck());
            players.Add(player);
        }

        PlayersInitializationClientRpc(args.PlayerOrder);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void PlayersInitializationClientRpc(List<ulong> playerOrder) {
        int localClientPlayerIndex = -1;
        for(int i = 0; i < playerOrder.Count; i++) {
            if (playerOrder[i] == NetworkManager.Singleton.LocalClientId) {
                localClientPlayerIndex = i;
                break;
            }
        }
        if (localClientPlayerIndex == -1)
            throw new Exception("Local Client Id not contained in player order list");

        OnPlayersInitialization?.Invoke(this, new PlayersInitializedEventArgs(playerOrder.Count, localClientPlayerIndex));
        OnPlayersInitializationFinished?.Invoke(this, new PlayersInitializedEventArgs(playerOrder.Count, localClientPlayerIndex));
    }

    public List<Card> Temp_PopulateDeck() {
        List<Card> result = new List<Card>();
        int tempDeckSize = 40;
        int databaseCardCount = CardDatabase.Instance.Cards.Count;
        for (int i = 0; i < tempDeckSize; i++) {
            Card card = CardDatabase.Instance.GetCardByIndex(UnityEngine.Random.Range(0, databaseCardCount)).GenerateCardFromBase();
            result.Add(card);
        }
        return result;
    }

    public List<Card> Temp_PopulateDeckNull() {
        List<Card> result = new List<Card>();
        int tempDeckSize = 40;
        for (int i = 0; i < tempDeckSize; i++)
            result.Add(new NullCard());
        return result;
    }

    public void NextTurn() {
        Debug.Log("Current player index: " + currentPlayerTurnIndex);
        currentPlayerTurnIndex = ++currentPlayerTurnIndex % players.Count;
        OnNextPlayerTurn?.Invoke(this, new NextPlayerTurnEventArgs(GetCurrentPlayerTurn(), currentPlayerTurnIndex));
        if (currentPlayerTurnIndex == 0) {
            fullTurnCount++;
            OnNextFullTurn?.Invoke(this, new NextFullTurnEventArgs(fullTurnCount));
        }
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

    public MatchPlayer GetPlayerById(ulong playerId) {
        MatchPlayer result = null;
        foreach(MatchPlayer p in players) {
            if (p.PlayerId == playerId)
                result = p;
        }

        return result;
    }

    public void IncrementFullTurnCount() {
        fullTurnCount++;
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

    public List<MatchPlayer> Players { get { return players; } }

    public int FullTurnCount { get { return fullTurnCount; } }
}
