using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour {
    [SerializeField] private Transform deckOrigin;
    [SerializeField] private Transform handOrigin;
    [SerializeField] private List<GameObject> cardsInHand;
    [Header("Prefabs")]
    [SerializeField] private GameObject card;

    private Guid playerUuid;
    private float cardSpacing = 0.65f;

    private void Start() {
        DuelManager duelManager = FindFirstObjectByType<DuelManager>();
        if (duelManager == null)
            throw new Exception("Could not find DuelManager object");

        duelManager.OnDrawCard += DrawCard;
    }

    public void DrawCard(object sender, DrawCardEventArgs args) {
        if (playerUuid != args.Player.Uuid)
            return;

        GameObject drawnCard = Instantiate(card, handOrigin);
        drawnCard.transform.Rotate(90f, 0, 0);
        cardsInHand.Add(drawnCard);
        SpaceCards();
    }

    private void SpaceCards() {
        int cardCount = cardsInHand.Count;
        float handOffset = (cardCount - 1) * cardSpacing / 2;
        for (int i = 0; i < cardCount; i++) {
            Vector3 cardPosition = handOrigin.position;
            cardPosition.x += i * cardSpacing - handOffset;
            cardsInHand[i].transform.position = cardPosition;
        }
    }

    public Guid PlayerUuid { get { return playerUuid; } set { playerUuid = value; } }
}