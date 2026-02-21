using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayingFieldUI : MonoBehaviour {
    [SerializeField] private Transform creatureSlotOrigin;
    [SerializeField] private Transform spellSlotOrigin;
    [SerializeField] private Transform domainSlotOrigin;
    [SerializeField] private List<CreatureFieldCardUI> creatureCards;
    [SerializeField] private SpellFieldCardUI domainCard;
    [Header("Prefabs")]
    [SerializeField] private CreatureFieldCardUI creatureCardUIPrefab;
    [SerializeField] private SpellFieldCardUI spellCardUIPrefab;

    private float cardSpacing = 0.6f;

    public void PlayCreatureCard(CreatureCard card) {
        CreatureFieldCardUI creatureCardUI = Instantiate(creatureCardUIPrefab, creatureSlotOrigin);
        creatureCardUI.Init(card);
        creatureCards.Add(creatureCardUI);
        SpaceCards();
    }

    public void PlayDomainCard(SpellCard card) {
        SpellFieldCardUI domainCardUI = Instantiate(spellCardUIPrefab, domainSlotOrigin);
        domainCardUI.Init(card);
        domainCard = domainCardUI;
    }

    public void TapCreature(CreatureCard card) {
        foreach (CreatureFieldCardUI cardUI in creatureCards) {
            if (cardUI.CardUuid == card.Uuid) {
                cardUI.Tap();
                break;
            }
        }
    }

    public void UntapCreature(CreatureCard card) {
        foreach(CreatureFieldCardUI cardUI in creatureCards) {
            if(cardUI.CardUuid == card.Uuid) {
                cardUI.Untap();
                break;
            }
        }
    }

    public bool ContainsCreatureCard(CreatureFieldCardUI other) {
        foreach(CreatureFieldCardUI cardUI in creatureCards) {
            if(cardUI == other)
                return true;
        }

        return false;
    }

    public void SetSelectableCards(MatchPlayer player) {
        for (int i = 0; i < creatureCards.Count; i++) {
            if (player.Creatures.Count <= i)
                throw new Exception("Creature cards in model and view do not match");

            creatureCards[i].SetBorderVisibility(player.Creatures[i].CanAttack());
        }
    }

    private void SpaceCards() {
        int cardCount = creatureCards.Count;
        float handOffset = (cardCount - 1) * cardSpacing / 2;
        for (int i = 0; i < cardCount; i++) {
            Vector3 cardPosition = creatureSlotOrigin.position;
            cardPosition.x += i * cardSpacing - handOffset;
            creatureCards[i].transform.position = cardPosition;
        }
    }
}
