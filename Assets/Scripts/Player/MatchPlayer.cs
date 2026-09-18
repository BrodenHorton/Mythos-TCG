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

    // TODO: Figure out how to combine Draw card and Added card to hand events into one method
    public Card DrawCard() {
        if(deck.Count == 0)
            throw new Exception("Attempting to draw a card when the player has no cards in their deck");

        Card card = deck[deck.Count - 1];
        hand.Add(card);
        deck.RemoveAt(deck.Count - 1);
        EventBus.Instance.InvokeOnCardDrawn(playerId, card);
        return card;
    }

    public void AddCardToHand(Card card) {
        hand.Add(card);
        EventBus.Instance.InvokeOnCardAddedToHand(playerId, card);
    }

    public void PlayCardFromHand(Card card) {
        RemoveCardFromHand(card.Uuid);
        CurrentMana -= card.GetManaCost();
        PlayCard(card);
    }

    public void PlayCard(Card card) {
        if (card is CreatureCard creatureCard)
            PlayCreatureCard(creatureCard);
        else if (card is DomainCard domainCard)
            PlayDomainCard(domainCard);
        else if (card is SpellCard spellCard)
            PlaySpellCard(spellCard);
        else
            throw new Exception("Attempting to play an unrecognized card type");
    }

    public void PlayCreatureCard(CreatureCard card) {
        card.CreatureDestroyedCallback = OnCreatureDestroyCallback;
        creatures.Add(card);

        EventBus.Instance.InvokeOnCreatureCardPlayedFromHand(new PlayerCardEventArgs<CreatureCard>(playerId, card));
        EventBus.Instance.InvokeOnCreatureCardPlayedFromHandFinished(playerId, card);

        PlayerCardCancelableEventArgs<CreatureCard> args = new PlayerCardCancelableEventArgs<CreatureCard>(playerId, card);
        EventBus.Instance.InvokeOnSummoningSickness(args);
        card.HasSummoningSickness = !args.IsCanceled;
    }

    public void PlayDomainCard(DomainCard card) {
        domain = card;
        EventBus.Instance.InvokeOnDomainCardPlayedFromHand(playerId, card);
    }

    public void PlaySpellCard(SpellCard card) {
        EventBus.Instance.InvokeOnSpellCardPlayedFromHand(new PlayerCardEventArgs<SpellCard>(playerId, card));
        if (card.SpellType == SpellType.Instant)
            card.ExecuteSpell();
        else
            EventBus.Instance.InvokeOnSpellChainCardPlayed(new PlayerCardEventArgs<SpellCard>(playerId, card));
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

    public void ModifyLifePoints(int amt) {
        int previousLifePoints = lifePoints;
        lifePoints += amt;
        EventBus.Instance.InvokeOnLifePointsChanged(new LifePointsChangedEventArgs(playerId, previousLifePoints, lifePoints));
        EventBus.Instance.InvokeOnLifePointsChangedFinishedClientRpc(playerId, previousLifePoints, lifePoints);
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
            ManaChangedEventArgs args = new ManaChangedEventArgs(playerId, value);
            EventBus.Instance.InvokeOnManaCountChanged(args);
            currentMana = args.ManaCount;
            EventBus.Instance.InvokeOnManaCountChangedFinished(args);
            EventBus.Instance.InvokeOnPostManaCountChanged(playerId, currentMana);
        }
    }
}
