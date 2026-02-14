using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OpponentUI : ResourceUI {
    private void Start() {
        DuelManager duelManager = FindFirstObjectByType<DuelManager>();
        if (duelManager == null)
            throw new Exception("Could not find DuelManager object");

        duelManager.OnManaCountChanged += (sender, e) => {
            if (e.Player.Uuid == playerUuid)
                manaCount.text = e.CurrentMana.ToString();
        };

        duelManager.OnDrawCard += DrawCard;
    }

    public void DrawCard(object sender, DrawCardEventArgs args) {
        if (playerUuid != args.Player.Uuid)
            return;

        HandCardUI drawnCard = Instantiate(card, handOrigin);
        drawnCard.transform.Rotate(-90f, 0, 0);
        cardsInHand.Add(drawnCard);
        SpaceCards();
    }

    private void PlayCardFromHand(int cardIndex) {
        DuelManager duelManager = FindFirstObjectByType<DuelManager>();
        if (cardIndex < 0 || cardsInHand.Count <= cardIndex)
            throw new Exception("Invalid card selected in hand");

        HandCardUI handCardUI = cardsInHand[cardIndex];
        cardsInHand.RemoveAt(cardIndex);
        Destroy(handCardUI.gameObject);
        SpaceCards();
        duelManager.PlayCardInHand(playerUuid, cardIndex);
    }

    private void SpaceCards() {
        float cardSpacing = 0.32f;
        float cardVerticalOffset = 0.005f;
        float cardRotation = -1.6f;

        int cardCount = cardsInHand.Count;
        float handOffsetX = (cardCount - 1) * cardSpacing / 2;
        float centerCardPoint = (cardCount - 1) / 2f;
        float handRotation = (cardCount - 1) * cardRotation / 2;
        for (int i = 0; i < cardCount; i++) {
            cardsInHand[i].transform.position = handOrigin.position;
            float xOffset = (i * cardSpacing - handOffsetX);
            float zOffset = (float)Math.Floor(Math.Abs(i - centerCardPoint)) * cardVerticalOffset * 3f;
            Vector3 cardPosition = new Vector3(xOffset, i * 0.005f, zOffset);
            cardsInHand[i].transform.Translate(cardPosition, Space.World);
            cardsInHand[i].transform.eulerAngles = new Vector3(cardsInHand[i].transform.eulerAngles.x, 0f, cardsInHand[i].transform.eulerAngles.z);
            cardsInHand[i].transform.Rotate(new Vector3(0, i * cardRotation - handRotation, 0), Space.World);
        }
    }
}