using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DeckSimulation", menuName = "Scriptable Objects/Simulation/Deck")]
public class DeckSimulation : ScriptableObject {
    [SerializeField] private List<CardBase> cards;

    public List<Card> GenerateDeck(ulong playerId) {
        List<Card> result = new List<Card>();
        for (int i = 0; i < cards.Count; i++) {
            Card card = cards[i].GenerateCardFromBase(playerId);
            result.Add(card);
        }

        return result;
    }
}
