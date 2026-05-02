using System;
using System.Collections.Generic;

[Serializable]
public class MatchPlayer {
    private ulong playerId;
    private List<Card> deck;
    private List<Card> hand;
    private List<Card> discardPile;
    private List<CreatureCard> creatures;
    private DomainCard domain;
    private int lifePoints;
    private int currentMana;
    private int seriesWinCount;

    public MatchPlayer(ulong playerId, List<Card> deck) {
        this.playerId = playerId;
        this.deck = new List<Card>(deck);
        hand = new List<Card>();
        discardPile = new List<Card>();
        creatures = new List<CreatureCard>();
        domain = null;
        lifePoints = 20;
        currentMana = 0;
        seriesWinCount = 0;
    }

    public void ShuffleDeck() {
        deck.Shuffle();
    }

    public Card DrawCard() {
        if(deck.Count == 0) {
            TcgLogger.Log("Player has ran out of cards to draw");
            return null;
        }

        Card card = deck[deck.Count - 1];
        hand.Add(card);
        deck.RemoveAt(deck.Count - 1);
        EventBus.InvokeOnCardDrawn(this, new PlayerCardEventArgs<Card>(this, card));
        return card;
    }

    public void PlayCreatureCardFromHand(CreatureCard card, int handIndex) {
        RemoveCardFromHandAt(handIndex);
        CurrentMana -= card.GetManaCost();
        creatures.Add(card);
        EventBus.InvokeOnCreatureCardPlayedFromHand(this, new PlayCardFromHandEventArgs<CreatureCard>(this, card, handIndex));
    }

    public void PlayDomainCardFromHand(DomainCard card, int handIndex) {
        RemoveCardFromHandAt(handIndex);
        CurrentMana -= card.GetManaCost();
        domain = card;
        EventBus.InvokeOnDomainCardPlayedFromHand(this, new PlayCardFromHandEventArgs<DomainCard>(this, card, handIndex));
    }

    public void PlaySpellCardFromHand(SpellCard card, int handIndex) {
        RemoveCardFromHandAt(handIndex);
        CurrentMana -= card.GetManaCost();
        EventBus.InvokeOnSpellCardPlayedFromHand(this, new PlayCardFromHandEventArgs<SpellCard>(this, card, handIndex));
    }

    public void RemoveCardFromHandAt(int handIndex) {
        if (handIndex < 0 || handIndex >= hand.Count)
            throw new Exception("Attempting to remove card from hand with invalid handIndex: " + handIndex);

        Card card = hand[handIndex];
        hand.RemoveAt(handIndex);
        EventBus.InvokeOnCardRemovedFromHand(this, new CardRemovedFromHandEventArgs(this, card, handIndex));
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

    public void DamageLifePoints(int amt) {
        int temp = lifePoints;
        lifePoints -= amt;
        EventBus.InvokeOnLifePointsChanged(this, new LifePointsChangedEventArgs(this, lifePoints));
    }

    public void OnCreatureHealthChangedCallback(CreatureCard card) {
        EventBus.InvokeOnCreatureHealthChanged(this, new PlayerCardEventArgs<CreatureCard>(this, card));
    }

    public void OnCreatureDamagedCallback(CreatureCard card) {
        EventBus.InvokeOnCreatureDamaged(this, new PlayerCardEventArgs<CreatureCard>(this, card));
    }

    public void OnCreatureDestroyCallback(CreatureCard card) {
        EventBus.InvokeOnCreatureDestroyed(this, new PlayerCardEventArgs<CreatureCard>(this, card));
        creatures.Remove(card);
    }

    public ulong PlayerId { get { return playerId; } }

    public List<Card> Deck { get { return deck; } }

    public List<Card> Hand { get { return hand; } }

    public List<Card> DiscardPile { get { return discardPile; } }

    public List<CreatureCard> Creatures { get { return creatures; } }

    public DomainCard Domain { get { return domain; } }

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
