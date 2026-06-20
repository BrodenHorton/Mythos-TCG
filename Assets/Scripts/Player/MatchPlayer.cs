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

    public Card DrawCard() {
        if(deck.Count == 0)
            throw new Exception("Attempting to draw a card when the player has no cards in their deck");

        Card card = deck[deck.Count - 1];
        hand.Add(card);
        deck.RemoveAt(deck.Count - 1);
        EventBus.Instance.InvokeOnCardDrawn(playerId, card);
        return card;
    }

    public void PlayCreatureCardFromHand(CreatureCard card) {
        RemoveCardFromHand(card.Uuid);
        CurrentMana -= card.GetManaCost();
        card.CreatureDestroyedCallback = OnCreatureDestroyCallback;

        PlayerCardCancelableEventArgs<CreatureCard> args = new PlayerCardCancelableEventArgs<CreatureCard>(playerId, card);
        EventBus.Instance.InvokeOnEnteringFieldSummoningSickness(args);
        card.HasSummoningSickness = !args.IsCanceled;

        creatures.Add(card);
        EventBus.Instance.InvokeOnCreatureCardPlayedFromHand(playerId, card);
    }

    public void PlayDomainCardFromHand(DomainCard card) {
        RemoveCardFromHand(card.Uuid);
        CurrentMana -= card.GetManaCost();
        domain = card;
        EventBus.Instance.InvokeOnDomainCardPlayedFromHand(playerId, card);
    }

    public void PlaySpellCardFromHand(SpellCard card) {
        RemoveCardFromHand(card.Uuid);
        CurrentMana -= card.GetManaCost();
        EventBus.Instance.InvokeOnSpellCardPlayedFromHand(new PlayerCardEventArgs<SpellCard>(playerId, card));
    }

    public void RemoveCardFromHand(Guid cardUuid) {
        if (!ContainsHandCardeUuid(cardUuid))
            throw new Exception("Unable to find card in players hand uuid: " + cardUuid);

        Card card = GetHandCardByUuid(cardUuid);
        for(int i = 0; i < hand.Count; i++) {
            if (hand[i].Uuid == cardUuid) {
                hand.RemoveAt(i);
                break;
            }
        }
        EventBus.Instance.InvokeOnCardRemovedFromHand(playerId, card);
    }

    public void RemoveCardFromHandAt(int handIndex) {
        if (handIndex < 0 || handIndex >= hand.Count)
            throw new Exception("Attempting to remove card from hand with invalid handIndex: " + handIndex);

        Card card = hand[handIndex];
        hand.RemoveAt(handIndex);
        EventBus.Instance.InvokeOnCardRemovedFromHand(playerId, card);
    }

    public void ShuffleDeck() {
        deck.Shuffle();
    }

    // TODO: Update this method so an event is called when the life points are either damaged or healed
    public void ModifyLifePoints(int amt) {
        if (amt == 0)
            throw new Exception("Attempting to modify life points by 0");

        lifePoints += amt;
        EventBus.Instance.InvokeOnLifePointsChanged(playerId, lifePoints);
    }

    public void ClearSummoningSickness() {
        for(int i = 0; i < creatures.Count; i++) {
            if (creatures[i].HasSummoningSickness)
                creatures[i].HasSummoningSickness = false;
        }
    }

    public void OnCreatureDestroyCallback(CreatureCard card) {
        EventBus.Instance.InvokeOnCreatureDestroyed(new PlayerCardEventArgs<CreatureCard>(playerId, card));
        creatures.Remove(card);
        EventBus.Instance.InvokeOnCreatureDestroyedFinishedClientRpc(playerId, new CreatureCardPayload(card));
        EventBus.Instance.InvokeOnPostCreatureDestroyedClientRpc(playerId, new CreatureCardPayload(card));
    }

    public Card GetHandCardByUuid(Guid uuid) {
        for (int i = 0; i < hand.Count; i++) {
            if (hand[i].Uuid == uuid)
                return hand[i];
        }

        throw new Exception("Unable to find player hand card with a Uuid of: " + uuid);
    }

    public CreatureCard GetCreatureByUuid(Guid uuid) {
        for (int i = 0; i < creatures.Count; i++) {
            if (creatures[i].Uuid == uuid)
                return creatures[i];
        }

        throw new Exception("Unable to find player creature card with a Uuid of: " + uuid);
    }

    public bool ContainsHandCardeUuid(Guid uuid) {
        foreach (Card card in hand) {
            if (card.Uuid == uuid)
                return true;
        }

        return false;
    }

    public bool ContainsCreatureUuid(Guid uuid) {
        foreach (CreatureCard card in creatures) {
            if (card.Uuid == uuid)
                return true;
        }

        return false;
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
            EventBus.Instance.InvokeOnManaCountChanged(playerId, currentMana);
        }
    }
}
