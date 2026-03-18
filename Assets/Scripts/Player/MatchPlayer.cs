using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MatchPlayer {
    private List<Card> deck;
    private List<Card> hand;
    private List<Card> discardPile;
    private List<CreatureCard> creatures;
    private SpellCard domain;
    
    private int lifePoints;
    private int currentMana;
    private int seriesWinCount;
    private ulong playerId;

    public MatchPlayer(ulong playerId) {
        deck = new List<Card>();
        Temp_PopulateDeck();
        hand = new List<Card>();
        discardPile = new List<Card>();
        creatures = new List<CreatureCard>();
        domain = null;
        lifePoints = 20;
        currentMana = 1;
        seriesWinCount = 0;
        this.playerId = playerId;
    }

    public void Temp_PopulateDeck() {
        int tempDeckSize = 10;
        int databaseCardCount = CardDatabase.Instance.Cards.Count;
        for (int i = 0; i < tempDeckSize; i++) {
            Card card = CardDatabase.Instance.GetCardByIndex(UnityEngine.Random.Range(0, databaseCardCount)).GenerateCardFromBase();
            deck.Add(card);
        }
    }

    public void ShuffleDeck() {
        deck.Shuffle();
    }

    public Card DrawCard() {
        if(deck.Count == 0) {
            Debug.Log("Player has ran out of cards to draw");
            return null;
        }

        Card card = deck[deck.Count - 1];
        hand.Add(card);
        deck.RemoveAt(deck.Count - 1);
        card.Init(this);
        return card;
    }

    public void PlayCreatureCard(CreatureCard card) {
        CurrentMana -= card.GetManaCost();
        creatures.Add(card);
        EventBus.InvokeOnCreatureCardPlayed(this, new PlayerCreatureCardEventArgs(this, card));
    }

    public void PlaySpellCard(SpellCard card) {
        CurrentMana -= card.GetManaCost();
        //player.Spells.Add(card);
        //EventBus.InvokeOnSpellCardPlayed(this, new PlayerSpellCardEventArgs(player, card));
    }

    public void PlayDomainCard(SpellCard card) {
        CurrentMana -= card.GetManaCost();
        domain = card;
        EventBus.InvokeOnDomainCardPlayed(this, new PlayerSpellCardEventArgs(this, card));
    }

    public bool ContainsCreatureUuid(Guid uuid) {
        foreach(CreatureCard card in creatures) {
            if (card.Uuid == uuid)
                return true;
        }

        return false;
    }

    public CreatureCard GetCreatureByUuid(Guid uuid) {
        for(int i = 0; i < creatures.Count; i++) {
            if (creatures[i].Uuid == uuid)
                return creatures[i];
        }

        return null;
    }

    public void LifePointsDamage(int amt) {
        int temp = lifePoints;
        lifePoints -= amt;
        Debug.Log("Life Points " + temp + " -> " + lifePoints);
        EventBus.InvokeOnLifePointsChanged(this, new LifePointsChangedEventArgs(this, lifePoints));
    }

    public void RemoveCreatureFromPlay(CreatureCard card) {
        if (!ContainsCreatureUuid(card.Uuid))
            throw new Exception("Player does not contain specified creature card");

        creatures.Remove(card);
    }

    public ulong PlayerId { get { return playerId; } }

    public List<Card> Deck {  get { return deck; } }

    public List<Card> Hand { get { return hand; } }

    public List<Card> DiscardPile { get { return discardPile; } }

    public List<CreatureCard> Creatures { get { return creatures; } }

    public SpellCard Domain {
        get {
            return domain;
        }
        set {
            domain = value;
            EventBus.InvokeOnDomainCardPlayed(this, new PlayerSpellCardEventArgs(this, domain));
        }
    }

    public int LifePoints { get { return lifePoints; } }

    public int CurrentMana {
        get { 
            return currentMana;
        }
        set {
            currentMana = value;
            EventBus.InvokeOnManaCountChanged(this, new ManaChangedEventArgs(this, currentMana));
        }
    }
}
