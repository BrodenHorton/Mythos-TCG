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
    [SerializeField] protected CreatureHandCardUI creatureCard;
    [SerializeField] protected SpellHandCardUI spellCard;

    public void Init(MatchPlayer player) {
        SetLifePoints(player.LifePoints);
        SetManaCount(player.CurrentMana);
    }

    public void SetLifePoints(int lifePointsCount) {
        lifePoints.text = lifePointsCount.ToString();
    }

    public void SetManaCount(int manaCount) {
        this.manaCount.text = manaCount.ToString();
    }

    public bool ContainsCard(HandCardUI handCardUI) {
        foreach (HandCardUI card in cardsInHand) {
            if (card.Equals(handCardUI))
                return true;
        }

        return false;
    }

    public TextMeshPro LifePoints { get { return lifePoints; } }

    public TextMeshPro ManaCount { get { return manaCount; } }

    public List<HandCardUI> CardsInHand { get { return cardsInHand; } }
}
