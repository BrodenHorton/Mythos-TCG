using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayingFieldUI : MonoBehaviour {
    [SerializeField] private Transform creatureSlotOrigin;
    [SerializeField] private Transform spellSlotOrigin;
    [SerializeField] private Transform domainSlotOrigin;
    [SerializeField] private List<GameObject> creatureCards;
    [SerializeField] private List<GameObject> spellCards;
    [SerializeField] private GameObject domainCard;
    [Header("Prefabs")]
    [SerializeField] private GameObject card;

    private Guid playerUuid;
    private float cardSpacing = 0.5f;

    private void Awake() {
        DuelManager duelManager = FindFirstObjectByType<DuelManager>();
        if (duelManager == null)
            throw new Exception("Could not find DuelManager object");

        duelManager.OnPlayCreatureCard += PlayCreatureCard;
        duelManager.OnPlaySpellCard += PlaySpellCard;
        duelManager.OnPlayDomainCard += PlayDomainCard;
    }

    public void PlayCreatureCard(object sender, DrawCardEventArgs args) {
        if (playerUuid != args.Player.Uuid)
            return;

        GameObject creatureCard = Instantiate(card, creatureSlotOrigin);
        creatureCard.transform.Rotate(90f, 0, 0);
        creatureCards.Add(creatureCard);
        SpaceCards();
    }

    public void PlaySpellCard(object sender, DrawCardEventArgs args) {
        if (playerUuid != args.Player.Uuid)
            return;

        GameObject spellCard = Instantiate(card, spellSlotOrigin);
        spellCard.transform.Rotate(90f, 0, 0);
        spellCards.Add(spellCard);
    }

    public void PlayDomainCard(object sender, DrawCardEventArgs args) {
        if (playerUuid != args.Player.Uuid)
            return;

        GameObject domainCard = Instantiate(card, domainSlotOrigin);
        domainCard.transform.Rotate(90f, 0, 0);
        this.domainCard = domainCard;
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
