using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class DuelistUI : MonoBehaviour {
    [SerializeField] protected Transform deckOrigin;
    [SerializeField] protected Transform handOrigin;
    [SerializeField] protected TextMeshPro lifePoints;
    [SerializeField] protected TextMeshPro manaCount;
    [SerializeField] protected List<HandCardUI> cardsInHand;
    [Header("Prefabs")]
    [SerializeField] protected CreatureHandCardUI creatureCardPrefab;
    [SerializeField] protected SpellHandCardUI spellCardPrefab;
    [SerializeField] protected DomainHandCardUI domainCardPrefab;
    [SerializeField] protected NullHandCardUI nullCardPrefab;

    protected ulong playerId;

    public void Init(ulong playerId, int lifePoints, int manaCount) {
        this.playerId = playerId;
        SetLifePoints(lifePoints);
        SetManaCount(manaCount);
    }

    public void SetLifePoints(int lifePointsCount) {
        lifePoints.text = lifePointsCount.ToString();
    }

    public void SetManaCount(int manaCount) {
        this.manaCount.text = manaCount.ToString();
    }

    public abstract void DrawCard(CardPayload card);

    public abstract void RemoveCardFromHand(Guid cardUuid);

    public abstract void SetDefaultCardPositions();

    public bool ContainsCard(Guid cardUuid) {
        foreach (HandCardUI card in cardsInHand) {
            if (card.CardUuid == cardUuid)
                return true;
        }

        return false;
    }

    public bool ContainsCard(HandCardUI handCardUI) {
        foreach (HandCardUI card in cardsInHand) {
            if (card.Equals(handCardUI))
                return true;
        }

        return false;
    }

    public ulong PlayerId { get { return playerId; } }

    public TextMeshPro LifePoints { get { return lifePoints; } }

    public TextMeshPro ManaCount { get { return manaCount; } }

    public List<HandCardUI> CardsInHand { get { return cardsInHand; } }
}
