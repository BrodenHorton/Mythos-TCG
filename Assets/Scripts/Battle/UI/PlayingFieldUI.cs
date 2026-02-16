using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayingFieldUI : MonoBehaviour {
    [SerializeField] private Transform creatureSlotOrigin;
    [SerializeField] private Transform spellSlotOrigin;
    [SerializeField] private Transform domainSlotOrigin;
    [SerializeField] private List<CreatureFieldCardUI> creatureCards;
    [SerializeField] private List<SpellFieldCardUI> spellCards;
    [SerializeField] private SpellFieldCardUI domainCard;
    [Header("Prefabs")]
    [SerializeField] private CreatureFieldCardUI creatureCardUIPrefab;
    [SerializeField] private SpellFieldCardUI spellCardUIPrefab;

    private Guid playerUuid;
    private float cardSpacing = 0.5f;

    private void Awake() {
        DuelManager duelManager = FindFirstObjectByType<DuelManager>();
        if (duelManager == null)
            throw new Exception("Could not find DuelManager object");

        EventBus.OnCreatureCardPlayed += PlayCreatureCard;
        EventBus.OnSpellCardPlayed += PlaySpellCard;
        //duelManager.OnDomainCardPlayed += PlayDomainCard;
    }

    public void PlayCreatureCard(object sender, PlayCreatureCardEventArgs args) {
        if (playerUuid != args.Player.Uuid)
            return;

        CreatureFieldCardUI creatureCardUI = Instantiate(creatureCardUIPrefab, creatureSlotOrigin);
        creatureCardUI.Init(args.Card);
        creatureCardUI.transform.Rotate(90f, 0, 0);
        creatureCards.Add(creatureCardUI);
        SpaceCards();
    }

    public void PlaySpellCard(object sender, PlaySpellCardEventArgs args) {
        if (playerUuid != args.Player.Uuid)
            return;

        SpellFieldCardUI spellCardUI = Instantiate(spellCardUIPrefab, spellSlotOrigin);
        spellCardUI.Init(args.Card);
        spellCardUI.transform.Rotate(90f, 0, 0);
        spellCards.Add(spellCardUI);
    }

    /*public void PlayDomainCard(object sender, DrawCardEventArgs args) {
        if (playerUuid != args.Player.Uuid)
            return;

        SpellFieldCardUI domainCardUI = Instantiate(spellCardUIPrefab, domainSlotOrigin);
        domainCardUI.Init(args.Card);
        domainCardUI.transform.Rotate(90f, 0, 0);
        domainCard = domainCardUI;
    }*/

    private void SpaceCards() {
        int cardCount = creatureCards.Count;
        float handOffset = (cardCount - 1) * cardSpacing / 2;
        for (int i = 0; i < cardCount; i++) {
            Vector3 cardPosition = creatureSlotOrigin.position;
            cardPosition.x += i * cardSpacing - handOffset;
            creatureCards[i].transform.position = cardPosition;
        }
    }

    public Guid PlayerUuid { get { return playerUuid; } set { playerUuid = value; } }
}
