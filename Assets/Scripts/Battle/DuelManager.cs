using System;
using System.Collections.Generic;
using UnityEngine;

public class DuelManager : MonoBehaviour {
    public event EventHandler<DrawCardEventArgs> OnDrawCard;
    public event EventHandler<ManaChangedEventArgs> OnManaCountChanged;
    public event EventHandler<CardPlayedFromHandEventArgs> OnCardPlayedFromHand;
    public event EventHandler<DrawCardEventArgs> OnCreatureCardPlayed;
    public event EventHandler<DrawCardEventArgs> OnSpellCardPlayed;
    public event EventHandler<DrawCardEventArgs> OnDomainCardPlayed;

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

    }

    public void DrawCard(MatchPlayer player) {
        Card card = player.DrawCard();
        OnDrawCard?.Invoke(this, new DrawCardEventArgs(player));
    }

    public void SetCurrentMana(MatchPlayer player, int manaCount) {
        player.CurrentMana = manaCount;
        OnManaCountChanged?.Invoke(this, new ManaChangedEventArgs(player, manaCount));
    }

    public void SetStartOfTurnMana() {
        GetCurrentPlayerTurn().CurrentMana = fullTurnCount;
        OnManaCountChanged?.Invoke(this, new ManaChangedEventArgs(GetCurrentPlayerTurn(), fullTurnCount));
    }

    public void PlayCardInHand(int cardIndex) {
        MatchPlayer player = players[0];
        if (player.Hand.Count <= cardIndex)
            return;

        player.Hand[cardIndex].PlayCard(this);
        player.Hand.RemoveAt(cardIndex);
    }

    public void PlayCreatureCard(MatchPlayer player, CreatureCard card) {
        // TODO: Implement adding a creature card to the MatchPlayer's field
    }

    public void PlaySpellCard(MatchPlayer player, SpellCard card) {
        // TODO: Implement adding a spell card to the MatchPlayer's field
    }

    public void PlayDomainCard(MatchPlayer player, SpellCard card) {
        // TODO: Implement adding a domain card to the MatchPlayer's field
    }

    public void PlayCardFromHand(MatchPlayer player, Card card) {
        OnCardPlayedFromHand?.Invoke(this, new CardPlayedFromHandEventArgs(player, card));
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
