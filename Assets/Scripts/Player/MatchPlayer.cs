using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MatchPlayer {
    [SerializeField] private List<Card> deck;
    [SerializeField] private List<Card> hand;
    [SerializeField] private List<Card> discardPile;
    [SerializeField] private int lifePoints;
    [SerializeField] private int maxMana;
    [SerializeField] private int currentMana;
    [SerializeField] private int seriesWinCount;
}
