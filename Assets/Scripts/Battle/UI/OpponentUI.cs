using System;
using UnityEngine;

public class OpponentUI : ResourceUI {
    private void Start() {
        DuelManager duelManager = FindFirstObjectByType<DuelManager>();
        if (duelManager == null)
            throw new Exception("Could not find DuelManager object");

        EventBus.OnManaCountChanged += (sender, e) => {
            if (e.Player.Uuid == playerUuid)
                manaCount.text = e.CurrentMana.ToString();
        };

        EventBus.OnCreatureCardDrawn += DrawCreatureCard;
        EventBus.OnSpellCardDrawn += DrawSpellCard;
    }

    private void DrawCreatureCard(object sender, DrawCreatureCardEventArgs args) {
        if (playerUuid != args.Player.Uuid)
            return;

        CreatureHandCardUI cardUI = Instantiate(creatureCard, handOrigin);
        cardUI.Init(args.Card);
        cardUI.transform.Rotate(-90f, 0, 0);
        cardsInHand.Add(cardUI);
        SpaceCards();
    }

    private void DrawSpellCard(object sender, DrawSpellCardEventArgs args) {
        if (playerUuid != args.Player.Uuid)
            return;

        SpellHandCardUI cardUI = Instantiate(spellCard, handOrigin);
        cardUI.Init(args.Card);
        cardUI.transform.Rotate(-90f, 0, 0);
        cardsInHand.Add(cardUI);
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
        float cardSpacing = 0.34f;
        float cardVerticalOffset = 0.005f;
        float cardRotation = -1f;

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