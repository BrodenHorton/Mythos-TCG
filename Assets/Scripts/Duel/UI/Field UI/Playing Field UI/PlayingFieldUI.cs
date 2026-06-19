using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayingFieldUI : MonoBehaviour {
    [SerializeField] private Transform creatureSlotOrigin;
    [SerializeField] private Transform domainSlotOrigin;
    [SerializeField] private float cardSpacing;
    [Header("Field Cards")]
    [SerializeField] private List<CreatureFieldCardUI> creatures;
    [SerializeField] private DomainFieldCardUI domainCard;
    [Header("Prefabs")]
    [SerializeField] private CreatureFieldCardUI creatureCardUIPrefab;
    [SerializeField] private DomainFieldCardUI domainCardUIPrefab;

    private ulong playerId;

    private void Start() {
        FieldCardSelectionManager.Instance.OnReleaseCreatureFieldCardDragFinished += (sender, args) => {
            if(args.CardUI.PlayerId == playerId) 
                SetDefaultCardPositions();
        };
    }

    public void Init(ulong playerId) {
        this.playerId = playerId;
    }

    public void PlayCreatureCard(CreatureCardPayload card) {
        CreatureFieldCardUI creatureCardUI = Instantiate(creatureCardUIPrefab);
        creatureCardUI.transform.parent = creatureSlotOrigin;
        creatureCardUI.Init(playerId, card);
        AddCreatureFieldCard(creatureCardUI);
    }

    public void AddCreatureFieldCard(CreatureFieldCardUI cardUI) {
        creatures.Add(cardUI);
        cardUI.IsInCombatField = false;
        SetDefaultCardPositions();
    }

    public void PlayDomainCard(DomainCardPayload card) {
        DomainFieldCardUI domainCardUI = Instantiate(domainCardUIPrefab);
        domainCardUI.transform.parent = domainSlotOrigin;
        domainCardUI.transform.localPosition = Vector3.zero;
        domainCardUI.Init(playerId, card);
        domainCard = domainCardUI;
    }

    public void RemoveCreature(Guid cardUuid) {
        if (!ContainsCreature(cardUuid))
            throw new Exception("Attempting to remove creature that is not in the playing field");

        RemoveCreature(GetCreatureFieldCardUIBy(cardUuid));
    }

    public void RemoveCreature(CreatureFieldCardUI cardUI) {
        if(!creatures.Contains(cardUI))
            throw new Exception("Attempting to remove creature that is not in the playing field");

        creatures.Remove(cardUI);
        Destroy(cardUI.gameObject);
        SetDefaultCardPositions();
    }

    public CreatureFieldCardUI ReleaseCreature(Guid cardUuid) {
        if (!ContainsCreature(cardUuid))
            throw new Exception("Attempting to release creature that is not in the playing field");

        CreatureFieldCardUI cardUI = GetCreatureFieldCardUIBy(cardUuid);
        creatures.Remove(cardUI);
        SetDefaultCardPositions();
        return cardUI;
    }

    public void SetCardSelectable(Guid cardUuid) {
        if (!ContainsCreature(cardUuid))
            throw new Exception("Attempting to set selectable card border visibility to card that is not in the PlayUI hand");

        GetCreatureFieldCardUIBy(cardUuid).SetSelectable(true);
    }

    public void SetCardSelectableAll(bool isSelectable) {
        foreach (FieldCardUI cardUI in creatures)
            cardUI.SetSelectable(isSelectable);
    }

    protected void SetDefaultCardPositions() {
        int cardCount = creatures.Count;
        float handOffset = (cardCount - 1) * cardSpacing / 2;
        for (int i = 0; i < cardCount; i++) {
            FieldCardUI cardUI = creatures[i];
            cardUI.transform.localScale = Vector3.one;
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
