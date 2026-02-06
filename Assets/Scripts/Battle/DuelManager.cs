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
        for(int i = 0; i < 3; i++)
            DrawCard(GetCurrentPlayerTurn());
        for (int i = 0; i < 6; i++)
            PlayCreatureCard(GetCurrentPlayerTurn());
        PlaySpellCard(GetCurrentPlayerTurn());
        PlaySpellCard(GetCurrentPlayerTurn());
        PlayDomainCard(GetCurrentPlayerTurn());
    }

    public void DrawCard(MatchPlayer player) {
        Card card = player.DrawCard();
        OnDrawCard.Invoke(this, new DrawCardEventArgs(player, currentPlayerTurnIndex));
    }

    public void PlayCreatureCard(MatchPlayer player) {
        OnPlayCreatureCard.Invoke(this, new DrawCardEventArgs(player, currentPlayerTurnIndex));
    }

    public void PlaySpellCard(MatchPlayer player) {
        OnPlaySpellCard.Invoke(this, new DrawCardEventArgs(player, currentPlayerTurnIndex));
    }

    public void PlayDomainCard(MatchPlayer player) {
        OnPlayDomainCard.Invoke(this, new DrawCardEventArgs(player, currentPlayerTurnIndex));
    }

    public int GetPlayerCount() {
        return players.Count;
    }

    public MatchPlayer GetCurrentPlayerTurn() {
        return players[currentPlayerTurnIndex];
    }

    public int TurnCount { get { return turnCount; } }
}
