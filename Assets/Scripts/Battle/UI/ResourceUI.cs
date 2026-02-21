using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class ResourceUI : MonoBehaviour {
    [SerializeField] protected Transform deckOrigin;
    [SerializeField] protected Transform handOrigin;
    [SerializeField] protected TextMeshPro manaCount;
    [SerializeField] protected List<HandCardUI> cardsInHand;
    [Header("Prefabs")]
    [SerializeField] protected CreatureHandCardUI creatureCard;
    [SerializeField] protected SpellHandCardUI spellCard;

    public bool ContainsCard(HandCardUI handCardUI) {
        foreach (HandCardUI card in cardsInHand) {
            if (card.Equals(handCardUI))
                return true;
        }

        return false;
    }

    public TextMeshPro ManaCount { get { return manaCount; } }

    public List<HandCardUI> CardsInHand { get { return cardsInHand; } }
}
