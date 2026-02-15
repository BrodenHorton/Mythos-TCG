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
    [SerializeField] protected HandCardUI card;

    protected Guid playerUuid;

    public bool ContainsCard(HandCardUI handCardUI) {
        foreach (HandCardUI card in cardsInHand) {
            if (card.Equals(handCardUI))
                return true;
        }

        return false;
    }

    public Guid PlayerUuid { get { return playerUuid; } set { playerUuid = value; } }
}
