using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Deck {
    [SerializeField] private string deckName;
    [SerializeField] private List<CardBase> cards;
    [SerializeField] private List<Domain> domains;
}