using System;
using System.Collections.Generic;
using UnityEngine;

public class DuelManager : MonoBehaviour {
    public event EventHandler<DrawCardEventArgs> OnDrawCard;
    public event EventHandler<DrawCardEventArgs> OnPlayCreatureCard;
    public event EventHandler<DrawCardEventArgs> OnPlaySpellCard;
    public event EventHandler<DrawCardEventArgs> OnPlayDomainCard;

    [SerializeField] private List<MatchPlayer> players = new List<MatchPlayer>();

    private int currentPlayerTurnIndex;
    private int turnCount;

    private void Awake() {
        if(players.Count < 2)
            throw new Exception("Not enough players to start match.");

        currentPlayerTurnIndex = 0;
        turnCount = 0;
    }

    private void Start() {

    }

    public void DrawCard(MatchPlayer player) {
        Card card = player.DrawCard();
        OnDrawCard.Invoke(this, new DrawCardEventArgs(player));
    }

    public void PlayCreatureCard(MatchPlayer player) {
        OnPlayCreatureCard.Invoke(this, new DrawCardEventArgs(player));
    }

    public void PlaySpellCard(MatchPlayer player) {
        OnPlaySpellCard.Invoke(this, new DrawCardEventArgs(player));
    }

    public void PlayDomainCard(MatchPlayer player) {
        OnPlayDomainCard.Invoke(this, new DrawCardEventArgs(player));
    }

    public void NextTurn() {
        Debug.Log("Current player index: " + currentPlayerTurnIndex);
        currentPlayerTurnIndex = ++currentPlayerTurnIndex % players.Count;
    }

    public int GetPlayerCount() {
        return players.Count;
    }

    public MatchPlayer GetCurrentPlayerTurn() {
        return players[currentPlayerTurnIndex];
    }

    public MatchPlayer GetPlayerByUuid(Guid uuid) {
        MatchPlayer result = null;
        foreach(MatchPlayer p in players) {
            if (p.Uuid == uuid)
                result = p;
        }

        return result;
    }

    public List<MatchPlayer> Players { get { return players; } }

    public int TurnCount { get { return turnCount; } }
}
