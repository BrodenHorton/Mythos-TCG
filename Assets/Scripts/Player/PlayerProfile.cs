using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerProfile {
    [SerializeField] private List<CardBase> collection;
    [SerializeField] private List<Deck> decks;
    [SerializeField] private int GamesPlayedCount;
    [SerializeField] private int winCount;
}
