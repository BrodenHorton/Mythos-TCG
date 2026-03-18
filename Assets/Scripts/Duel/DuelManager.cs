using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DuelManager : MonoBehaviour {
    public event EventHandler OnPlayersInitialized;
    public event EventHandler<NextPlayerTurnEventArgs> OnNextPlayerTurn;
    public event EventHandler<NextFullTurnEventArgs> OnNextFullTurn;

    private List<MatchPlayer> players;
    private MatchPlayer localClientPlayer;
    private int currentPlayerTurnIndex;
    private int fullTurnCount;

    private void Awake() {
        currentPlayerTurnIndex = 0;
        fullTurnCount = 4;
    }

    private void Start() {
        GameManager.Instance.OnGameStart += InitializePlayers;
    }

    public void InitializePlayers(object sender, StartGameEventArgs args) {
        players = new List<MatchPlayer>();
        foreach(ulong playerId in NetworkManager.Singleton.ConnectedClients.Keys) {
            MatchPlayer player = new MatchPlayer(playerId);
            if (playerId == NetworkManager.Singleton.LocalClientId) {
                localClientPlayer = player;
            }
            players.Add(player);
        }
        OnPlayersInitialized?.Invoke(this, EventArgs.Empty);
    }

    public void PlayCardFromHand(MatchPlayer player, int cardIndex) {
        if (player.Hand.Count <= cardIndex)
            return;

        Card card = player.Hand[cardIndex];
        player.Hand.RemoveAt(cardIndex);
        card.PlayCard(this, player);
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
