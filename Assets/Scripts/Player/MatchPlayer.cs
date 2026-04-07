using System;
using System.Collections.Generic;

[Serializable]
public class MatchPlayer {
    private ulong playerId;
    private List<Card> deck;
    private List<Card> hand;
    private List<Card> discardPile;
    private List<CreatureCard> creatures;
    private SpellCard domain;
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
        EventBus.InvokeOnCardDrawn(this, new PlayerCardEventArgs(this, card));
        return card;
    }

    public void PlayCreatureCardFromHand(CreatureCard card, int handIndex) {
        RemoveCardFromHandAt(handIndex);
        CurrentMana -= card.GetManaCost();
        creatures.Add(card);
        EventBus.InvokeOnCreatureCardPlayedFromHand(this, new PlayCreatureCardFromHandEventArgs(this, card, handIndex));
    }

    public void PlaySpellCard(SpellCard card) {
        CurrentMana -= card.GetManaCost();
        //player.Spells.Add(card);
    }

    public void PlayDomainCard(SpellCard card) {
        CurrentMana -= card.GetManaCost();
        domain = card;
        EventBus.InvokeOnDomainCardPlayed(this, new PlayerSpellCardEventArgs(this, card));
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

    public void LifePointsDamage(int amt) {
        int temp = lifePoints;
        lifePoints -= amt;
        EventBus.InvokeOnLifePointsChanged(this, new LifePointsChangedEventArgs(this, lifePoints));
    }

    public void RemoveCreatureFromPlay(CreatureCard card) {
        if (!ContainsCreatureUuid(card.Uuid))
            throw new Exception("Player does not contain specified creature card");

        TcgLogger.Log("Creature Removed From Play");
        creatures.Remove(card);
        EventBus.InvokeOnCreatureDestroyed(this, new PlayerCreatureCardEventArgs(this, card));
    }

    public ulong PlayerId { get { return playerId; } }

    public List<Card> Deck { get { return deck; } }

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
