using System;
using System.Collections.Generic;
using UnityEngine;

public class DuelManager : MonoBehaviour {
    public event EventHandler<DrawCardEventArgs> OnDrawCard;
    public event EventHandler<EventArgs> OnPlayCreatureCard;
    public event EventHandler<EventArgs> OnPlaySpellCard;
    public event EventHandler<EventArgs> OnPlayDomainCard;

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
        for(int i = 0; i < 3; i++)
            DrawCard(players[0]);
        for (int i = 0; i < 6; i++)
            PlayCreatureCard(players[0]);
        PlaySpellCard(players[0]);
        PlaySpellCard(players[0]);
        PlayDomainCard(players[0]);
    }

    public void DrawCard(MatchPlayer player) {
        Card card = player.DrawCard();
        OnDrawCard.Invoke(this, new DrawCardEventArgs(currentPlayerTurn));
    }

    public void PlayCreatureCard(MatchPlayer player) {
        OnPlayCreatureCard.Invoke(this, EventArgs.Empty);
    }

    public void PlaySpellCard(MatchPlayer player) {
        OnPlaySpellCard.Invoke(this, EventArgs.Empty);
    }

    public void PlayDomainCard(MatchPlayer player) {
        OnPlayDomainCard.Invoke(this, EventArgs.Empty);
    }

    public int GetPlayerCount() {
        return players.Count;
    }

    public MatchPlayer CurrentPlayerTurn { get { return currentPlayerTurn; } }

    public int TurnCount { get { return turnCount; } }
}
