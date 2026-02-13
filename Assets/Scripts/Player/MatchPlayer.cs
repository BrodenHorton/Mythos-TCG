using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MatchPlayer {
    [SerializeReference, SubclassSelector] private List<Card> deck;
    [SerializeReference, SubclassSelector] private List<Card> hand;
    [SerializeReference, SubclassSelector] private List<Card> discardPile;
    [SerializeField] private int lifePoints;
    [SerializeField] private int currentMana;
    [SerializeField] private int seriesWinCount;

    private Guid uuid;

    public MatchPlayer() {
        deck = new List<Card>();
        hand = new List<Card>();
        discardPile = new List<Card>();
        lifePoints = 20;
        currentMana = 1;
        seriesWinCount = 0;
        uuid = Guid.NewGuid();
    }

    public Card DrawCard() {
        if(deck.Count == 0) {
            Debug.Log("Player has ran out of cards to draw");
            return null;
        }

        Card card = deck[deck.Count - 1];
        hand.Add(card);
        deck.RemoveAt(deck.Count - 1);
        return card;
    }

    public Guid Uuid { get { return uuid; } }

    public List<Card> Deck {  get { return deck; } }

    public List<Card> Hand { get { return hand; } }

    public List<Card> DiscardPile { get { return discardPile; } }

    public int CurrentMana { get { return currentMana; } set { currentMana = value; } }
}
