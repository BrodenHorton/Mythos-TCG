using System;
using System.Collections.Generic;
using UnityEngine;

public class DuelManager : MonoBehaviour {
    [SerializeField] private List<MatchPlayer> players = new List<MatchPlayer>();

    private int playerTurnIndex;
    private int turnCount;

    private void Awake() {
        if(players.Count < 2)
            throw new Exception("Not enough players to start match.");

        playerTurnIndex = 0;
        turnCount = 0;
    }

    private void Start() {
        
    }

    public int GetPlayerCount() {
        return players.Count;
    }
}
