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
        DuelStateManager stateManager = FindFirstObjectByType<DuelStateManager>();
        if (stateManager == null)
            throw new Exception("CJould not find DuelStateMangaer object");

        stateManager.DrawPhase.OnDrawPhase += UntapCreatures;
        EventBus.OnCreatureCardPlayed += PlayCreatureCard;
        EventBus.OnSpellCardPlayed += PlaySpellCard;
        EventBus.OnDomainCardPlayed += PlayDomainCard;
    }

    public void PlayCreatureCard(object sender, PlayCreatureCardEventArgs args) {
        if (playerUuid != args.Player.Uuid)
            return;

        CreatureFieldCardUI creatureCardUI = Instantiate(creatureCardUIPrefab, creatureSlotOrigin);
        creatureCardUI.Init(args.Card);
        creatureCardUI.Tap();
        creatureCards.Add(creatureCardUI);
        SpaceCards();
    }

    public void PlaySpellCard(object sender, PlaySpellCardEventArgs args) {
        if (playerUuid != args.Player.Uuid)
            return;

        SpellFieldCardUI spellCardUI = Instantiate(spellCardUIPrefab, spellSlotOrigin);
        spellCardUI.Init(args.Card);
        spellCards.Add(spellCardUI);
    }

    public void PlayDomainCard(object sender, PlaySpellCardEventArgs args) {
        if (playerUuid != args.Player.Uuid)
            return;

        SpellFieldCardUI domainCardUI = Instantiate(spellCardUIPrefab, domainSlotOrigin);
        domainCardUI.Init(args.Card);
        domainCard = domainCardUI;
    }

    public void UntapCreatures(object sender, EventArgs args) {
        foreach(CreatureFieldCardUI cardUI in creatureCards)
            cardUI.UnTap();
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

    public Guid PlayerUuid { get { return playerUuid; } set { playerUuid = value; } }
}
