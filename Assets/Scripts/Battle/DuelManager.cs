using System;
using System.Collections.Generic;
using UnityEngine;

public class DuelManager : MonoBehaviour {
    [SerializeField] private List<MatchPlayer> players = new List<MatchPlayer>();

    private int currentPlayerTurnIndex;
    private int fullTurnCount;

    private void Awake() {
        if(players.Count < 2)
            throw new Exception("Not enough players to start match.");

        currentPlayerTurnIndex = 0;
        fullTurnCount = 1;
    }

    private void Start() {
        InitializePlayers();
    }

    public void InitializePlayers() {
        foreach(MatchPlayer player in players) {
            player.ShuffleDeck();
            for (int i = 0; i < 5; i++)
                player.DrawCard();
        }
    }

    public void DrawCard(MatchPlayer player) {
        Debug.Log("Drawing Card in DuelManager");
        Card card = player.DrawCard();
    }

    public void SetCurrentMana(MatchPlayer player, int manaCount) {
        player.CurrentMana = manaCount;
    }

    public void SetStartOfTurnMana() {
        GetCurrentPlayerTurn().CurrentMana = fullTurnCount;
    }

    public void PlayCardInHand(Guid playerUuid, int cardIndex) {
        PlayCardInHand(GetPlayerByUuid(playerUuid), cardIndex);
    }

    public void PlayCardInHand(MatchPlayer player, int cardIndex) {
        Debug.Log("PlayCardInHand method called");
        if (player.Hand.Count <= cardIndex)
            return;

        Card card = player.Hand[cardIndex];
        player.Hand.RemoveAt(cardIndex);
        card.PlayCard(this, player);
    }

    public void PlayCreatureCard(MatchPlayer player, CreatureCard card) {
        player.Creatures.Add(card);
        EventBus.InvokeOnCreatureCardPlayed(this, new PlayCreatureCardEventArgs(player, card));
    }

    public void PlaySpellCard(MatchPlayer player, SpellCard card) {
        player.Spells.Add(card);
        EventBus.InvokeOnSpellCardPlayed(this, new PlaySpellCardEventArgs(player, card));
    }

    public void PlayDomainCard(MatchPlayer player, SpellCard card) {
        player.Domain = card;
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
