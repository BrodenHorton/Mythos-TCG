using System;
using System.Collections.Generic;
using UnityEngine;

public class DuelManager : MonoBehaviour {
    [SerializeField] private List<MatchPlayer> players = new List<MatchPlayer>();

    private MatchPlayer currentPlayerTurn;
    private int turnCount;

    private void Awake() {
        if(players.Count < 2)
            throw new Exception("Not enough players to start match.");

        currentPlayerTurn = players[0];
        turnCount = 0;
    }

    private void Start() {
        
    }

    public void DrawCard(MatchPlayer player) {
        player.DrawCard();
    }

    public int GetPlayerCount() {
        return players.Count;
    }

    public MatchPlayer CurrentPlayerTurn { get { return currentPlayerTurn; } }

    public int TurnCount { get { return turnCount; } }
}
