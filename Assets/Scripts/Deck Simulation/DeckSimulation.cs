using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DeckSimulation", menuName = "Scriptable Objects/Simulation/Deck")]
public class DeckSimulation : ScriptableObject {
    [SerializeField] private List<CardBase> cards;

    public List<Card> GenerateDeck() {
        List<Card> result = new List<Card>();
        for (int i = 0; i < cards.Count; i++) {
            Card card = cards[i].GenerateCardFromBase();
            result.Add(card);
        }

        return result;
    }
}
