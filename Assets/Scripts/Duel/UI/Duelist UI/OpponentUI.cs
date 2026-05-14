using System;
using UnityEngine;

public class OpponentUI : DuelistUI {

    public override void DrawCard(Card card) {
        NullHandCardUI cardUI = Instantiate(nullCardPrefab, handOrigin);
        cardUI.transform.Rotate(-90f, 0, 0);
        cardsInHand.Add(cardUI);
        SetDefaultCardPositions();
    }

    public override void RemoveCardFromHand(Guid cardUuid) {
        if (cardsInHand.Count == 0)
            throw new Exception("Attempting to remove cardUI from opponents hand when hand is empty");

        HandCardUI cardUI = cardsInHand[0];
        cardsInHand.RemoveAt(0);
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
