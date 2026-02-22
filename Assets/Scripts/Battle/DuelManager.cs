using System;
using System.Collections.Generic;
using UnityEngine;

public class DuelManager : MonoBehaviour {
    [SerializeField] private List<MatchPlayer> players = new List<MatchPlayer>();
    [SerializeField] private int initialHandSize;

    private MatchPlayer activePlayer;
    private int currentPlayerTurnIndex;
    private int fullTurnCount;

    private void Awake() {
        if(players.Count < 2)
            throw new Exception("Not enough players to start match.");

        activePlayer = players[0];
        currentPlayerTurnIndex = 0;
        fullTurnCount = 4;
    }

    private void Start() {
        InitializePlayers();
    }

    public void InitializePlayers() {
        foreach(MatchPlayer player in players) {
            player.ShuffleDeck();
            for (int i = 0; i < initialHandSize; i++)
                player.DrawCard();
        }
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
        if (currentPlayerTurnIndex == 0)
            fullTurnCount++;
    }

    public int GetPlayerCount() {
        return players.Count;
    }

    public MatchPlayer GetCurrentPlayerTurn() {
        return players[currentPlayerTurnIndex];
    }

    public bool IsActivePlayerTurn() {
        return activePlayer == GetCurrentPlayerTurn();
    }

    public int GetStartOfTurnManaCount() {
        return fullTurnCount;
    }

    public MatchPlayer GetPlayerByUuid(Guid uuid) {
        MatchPlayer result = null;
        foreach(MatchPlayer p in players) {
            if (p.Uuid == uuid)
                result = p;
        }

        return result;
    }

    public void IncrementFullTurnCount() {
        fullTurnCount++;
    }

    public List<MatchPlayer> Players { get { return players; } }

    public int FullTurnCount { get { return fullTurnCount; } }
}
