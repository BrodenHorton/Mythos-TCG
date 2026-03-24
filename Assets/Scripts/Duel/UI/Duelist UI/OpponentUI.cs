using System;
using UnityEngine;

public class OpponentUI : DuelistUI {

    public override void DrawCard(Card card) {
        if (card is CreatureCard creatureCard) {
            CreatureHandCardUI cardUI = Instantiate(creatureHandCardPrefab, handOrigin);
            cardUI.Init(creatureCard);
            cardUI.transform.Rotate(-90f, 0, 0);
            cardsInHand.Add(cardUI);
        }
        else if (card is SpellCard spellCard) {
            SpellHandCardUI cardUI = Instantiate(spellHandCardPrefab, handOrigin);
            cardUI.Init(spellCard);
            cardUI.transform.Rotate(-90f, 0, 0);
            cardsInHand.Add(cardUI);
        }

        SetDefaultCardPositions();
    }

    public override void RemoveCardFromHand(int handIndex) {
        if (handIndex < 0 || handIndex >= cardsInHand.Count)
            throw new Exception("Attempting to remove cardUI from hand with invalid handIndex: " + handIndex);

        HandCardUI cardUI = cardsInHand[handIndex];
        cardsInHand.RemoveAt(handIndex);
        Destroy(cardUI.gameObject);
        SetDefaultCardPositions();
    }

    public override void SetDefaultCardPositions() {
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
