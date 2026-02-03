using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MatchPlayer {
    [SerializeField] private List<Card> deck;
    [SerializeField] private List<Card> hand;
    [SerializeField] private List<Card> discardPile;
    [SerializeField] private int lifePoints;
    [SerializeField] private int maxMana;
    [SerializeField] private int currentMana;
    [SerializeField] private int seriesWinCount;

    public MatchPlayer() {
        deck = new List<Card>();
        hand = new List<Card>();
        discardPile = new List<Card>();
        lifePoints = 20;
        maxMana = 1;
        currentMana = 1;
        seriesWinCount = 0;
    }

    public void DrawCard() {
        if(deck.Count == 0) {
            Debug.Log("Player has ran out of cards to draw");
            return;
        }

        hand.Add(deck[deck.Count - 1]);
        deck.RemoveAt(deck.Count - 1);
    }
}
