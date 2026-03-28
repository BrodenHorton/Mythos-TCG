using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DuelManager : NetworkBehaviour {
    public event EventHandler<PlayersInitializedEventArgs> OnPlayersInitialization;
    public event EventHandler<PlayersInitializedEventArgs> OnPlayersInitializationFinished;
    public event EventHandler<NextPlayerTurnEventArgs> OnNextPlayerTurn;
    public event EventHandler<NextFullTurnEventArgs> OnNextFullTurn;

    private List<MatchPlayer> players;
    private MatchPlayer localClientPlayer;
    private int currentPlayerTurnIndex;
    private int fullTurnCount;

    private void Awake() {
        currentPlayerTurnIndex = 0;
        fullTurnCount = 1;
    }

    private void Start() {
        GameManager.Instance.OnGameStart += InitializePlayers;
        EventBus.OnCreatureCardSelectedForPlay += PlayCreatureCard;
    }

    public void InitializePlayers(object sender, StartGameEventArgs args) {
        players = new List<MatchPlayer>();
        for(int i = 0; i < args.PlayerOrder.Count; i++) {
            MatchPlayer player;
            if (args.PlayerOrder[i] == NetworkManager.Singleton.LocalClientId) {
                player = new MatchPlayer(args.PlayerOrder[i], Temp_PopulateDeck());
                localClientPlayer = player;
            }
            else 
                player = new MatchPlayer(args.PlayerOrder[i], Temp_PopulateDeckNull());
            players.Add(player);
        }
        if (localClientPlayer == null)
            throw new Exception("Local Client Id not found in start game player list");

        OnPlayersInitialization?.Invoke(this, new PlayersInitializedEventArgs(players.Count, GetPlayerIndex(localClientPlayer)));
        OnPlayersInitializationFinished?.Invoke(this, new PlayersInitializedEventArgs(players.Count, GetPlayerIndex(localClientPlayer)));
    }

    private List<Card> Temp_PopulateDeck() {
        List<Card> result = new List<Card>();
        int tempDeckSize = 40;
        int databaseCardCount = CardDatabase.Instance.Cards.Count;
        for (int i = 0; i < tempDeckSize; i++) {
            Card card = CardDatabase.Instance.GetCardByIndex(UnityEngine.Random.Range(0, databaseCardCount)).GenerateCardFromBase();
            result.Add(card);
        }
        return result;
    }

    private List<Card> Temp_PopulateDeckNull() {
        List<Card> result = new List<Card>();
        int tempDeckSize = 40;
        int databaseCardCount = CardDatabase.Instance.Cards.Count;
        for (int i = 0; i < tempDeckSize; i++)
            result.Add(new NullCard());
        return result;
    }

    public void PlayCardFromHand(MatchPlayer player, int handIndex) {
        if (player.Hand.Count <= handIndex)
            throw new Exception("Attmepting to play card with invalid hadnIndex: " + handIndex);

        player.Hand[handIndex].PlayCardFromHand(player, handIndex);
    }

    public void PlayCreatureCard(object sender, PlayCreatureCardFromHandEventArgs args) {
        PlayCreatureCardFromHandServerRpc(GetPlayerIndex(args.Player.PlayerId), args.Card.GetNetworkSerializableObject(), args.HandIndex);
    }

    [Rpc(SendTo.Server)]
    private void PlayCreatureCardFromHandServerRpc(int playerIndex, CreatureCardNetworkSerializable cardNetworkSerializableObject, int handIndex) {
        PlayCreatureCardFromHandClientRpc(playerIndex, cardNetworkSerializableObject, handIndex);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void PlayCreatureCardFromHandClientRpc(int playerIndex, CreatureCardNetworkSerializable cardNetworkSerializableObject, int handIndex) {
        MatchPlayer player = Players[playerIndex];
        CreatureCard card = new CreatureCard(cardNetworkSerializableObject);
        player.PlayCreatureCardFromHand(card, handIndex);
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

    public bool IsLocalClientPlayerTurn() {
        return localClientPlayer == GetCurrentPlayerTurn();
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

    public MatchPlayer LocalClientPlayer { get { return localClientPlayer; } }

    public int FullTurnCount { get { return fullTurnCount; } }
}
