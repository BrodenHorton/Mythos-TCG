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
}