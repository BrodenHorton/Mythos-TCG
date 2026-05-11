using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayingFieldUI : MonoBehaviour {
    [SerializeField] protected Transform creatureSlotOrigin;
    [SerializeField] protected Transform domainSlotOrigin;
    [SerializeField] protected float cardSpacing;
    [Header("Field Cards")]
    [SerializeField] protected List<CreatureFieldCardUI> creatures;
    [SerializeField] protected DomainFieldCardUI domainCard;
    [Header("Prefabs")]
    [SerializeField] protected CreatureFieldCardUI creatureCardUIPrefab;
    [SerializeField] protected SpellFieldCardUI spellCardUIPrefab;
    [SerializeField] protected DomainFieldCardUI domainCardUIPrefab;

    protected ulong playerId;

    public void Init(ulong playerId) {
        this.playerId = playerId;
    }

    public void PlayCreatureCard(CreatureCard card) {
        CreatureFieldCardUI creatureCardUI = Instantiate(creatureCardUIPrefab);
        creatureCardUI.transform.parent = creatureSlotOrigin;
        creatureCardUI.Init(card);
        AddCreatureFieldCard(creatureCardUI);
    }

    public void AddCreatureFieldCard(CreatureFieldCardUI cardUI) {
        creatures.Add(cardUI);
        SetDefaultCardPositions();
    }

    public void PlayDomainCard(DomainCard card) {
        DomainFieldCardUI domainCardUI = Instantiate(domainCardUIPrefab);
        domainCardUI.transform.parent = domainSlotOrigin;
        domainCardUI.transform.localPosition = Vector3.zero;
        domainCardUI.Init(card);
        domainCard = domainCardUI;
    }

    public void UpdateCreatureFieldCard(CreatureCard card) {
        if (!ContainsCreature(card.Uuid))
            throw new Exception("Attempting to update creature field card that is not in the playing field");

        GetCreatureFieldCardUIBy(card.Uuid).UpdateCreatureFieldCard(card);
    }

    public void TapCreature(CreatureCard card) {
        foreach (CreatureFieldCardUI cardUI in creatures) {
            if (cardUI.CardUuid == card.Uuid) {
                cardUI.Tap();
                return;
            }
        }

        throw new Exception("Attempting to tap creature that is not in the playing field");
    }

    public void UntapCreature(CreatureCard card) {
        foreach (CreatureFieldCardUI cardUI in creatures) {
            if (cardUI.CardUuid == card.Uuid) {
                cardUI.Untap();
                return;
            }
        }

        throw new Exception("Attempting to untap creature that is not in the playing field");
    }

    public void RemoveCreature(Guid uuid) {
        foreach (CreatureFieldCardUI cardUI in creatures) {
            if (cardUI.CardUuid == uuid) {
                RemoveCreature(cardUI);
                return;
            }
        }

        throw new Exception("Attempting to remove creature that is not in the playing field");
    }

    public void RemoveCreature(CreatureFieldCardUI cardUI) {
        if(!creatures.Contains(cardUI))
            throw new Exception("Attempting to remove creature that is not in the playing field");

        creatures.Remove(cardUI);
        Destroy(cardUI.gameObject);
        SetDefaultCardPositions();
    }

    protected void SetDefaultCardPositions() {
        int cardCount = creatures.Count;
        float handOffset = (cardCount - 1) * cardSpacing / 2;
        for (int i = 0; i < cardCount; i++) {
            FieldCardUI cardUI = creatures[i];
            cardUI.transform.localScale = Vector3.one;
            cardUI.transform.eulerAngles = Vector3.zero;
            Vector3 cardPosition = creatureSlotOrigin.position;
            cardPosition.x += i * cardSpacing - handOffset;
            cardUI.transform.position = cardPosition;
        }
    }

    public bool ContainsCreature(Guid cardUuid) {
        foreach (CreatureFieldCardUI cardUI in creatures) {
            if (cardUI.CardUuid == cardUuid)
                return true;
        }

        return false;
    }

    public bool ContainsCreature(CreatureFieldCardUI other) {
        foreach (CreatureFieldCardUI cardUI in creatures) {
            if (cardUI == other)
                return true;
        }

        return false;
    }

    public CreatureFieldCardUI GetCreatureFieldCardUIBy(Guid uuid) {
        foreach (CreatureFieldCardUI cardUI in creatures) {
            if (cardUI.CardUuid == uuid)
                return cardUI;
        }

        throw new Exception("Unable to find creature field card with uuid: " + uuid);
    }

    public ulong PlayerId { get { return playerId; } }
}
