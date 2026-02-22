using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayingFieldUI : MonoBehaviour {
    [SerializeField] private Transform creatureSlotOrigin;
    [SerializeField] private Transform domainSlotOrigin;
    [SerializeField] private List<CreatureFieldCardUI> creatures;
    [SerializeField] private SpellFieldCardUI domainCard;
    [Header("Prefabs")]
    [SerializeField] private CreatureFieldCardUI creatureCardUIPrefab;
    [SerializeField] private SpellFieldCardUI spellCardUIPrefab;

    private float cardSpacing = 0.6f;

    public void PlayCreatureCard(CreatureCard card) {
        CreatureFieldCardUI creatureCardUI = Instantiate(creatureCardUIPrefab, creatureSlotOrigin);
        creatureCardUI.Init(card);
        creatures.Add(creatureCardUI);
        SpaceCards();
    }

    public void PlayDomainCard(SpellCard card) {
        SpellFieldCardUI domainCardUI = Instantiate(spellCardUIPrefab, domainSlotOrigin);
        domainCardUI.Init(card);
        domainCard = domainCardUI;
    }

    public void TapCreature(CreatureCard card) {
        foreach (CreatureFieldCardUI cardUI in creatures) {
            if (cardUI.CardUuid == card.Uuid) {
                cardUI.Tap();
                break;
            }
        }
    }

    public void UntapCreature(CreatureCard card) {
        foreach(CreatureFieldCardUI cardUI in creatures) {
            if(cardUI.CardUuid == card.Uuid) {
                cardUI.Untap();
                break;
            }
        }
    }

    public void RemoveCreature(CreatureFieldCardUI cardUI) {
        creatures.Remove(cardUI);
        Destroy(cardUI.gameObject);
    }

    public void RemoveCreatureAndUpdate(CreatureFieldCardUI cardUI) {
        creatures.Remove(cardUI);
        Destroy(cardUI);
        SpaceCards();
    }

    public bool ContainsBenchCreature(CreatureFieldCardUI other) {
        foreach(CreatureFieldCardUI cardUI in creatures) {
            if(cardUI == other)
                return true;
        }

        return false;
    }

    public void SetSelectableCards(MatchPlayer player) {
        for (int i = 0; i < creatures.Count; i++) {
            if (player.Creatures.Count <= i)
                throw new Exception("Creature cards in model and view do not match");

            creatures[i].SetBorderVisibility(player.Creatures[i].CanAttack());
        }
    }

    private void SpaceCards() {
        int cardCount = creatures.Count;
        float handOffset = (cardCount - 1) * cardSpacing / 2;
        for (int i = 0; i < cardCount; i++) {
            Vector3 cardPosition = creatureSlotOrigin.position;
            cardPosition.x += i * cardSpacing - handOffset;
            creatures[i].transform.position = cardPosition;
        }
    }
}
