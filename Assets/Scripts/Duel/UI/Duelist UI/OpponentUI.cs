using System;
using UnityEngine;

public class OpponentUI : DuelistUI {

    public override void DrawCard(CardPayload card) {
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
        float radius = 40f;
        float arcDistanceInterval = 1.15f;
        // TODO: Figure out how to detect which axis and direction the radius should be added to so you get the correct circle center
        Vector3 circleCenter = new Vector3(handOrigin.position.x,
                                           handOrigin.position.y,
                                           handOrigin.position.z + radius);
        int cardCount = cardsInHand.Count;
        float initialArcDistance = (cardCount - 1) * arcDistanceInterval / 2;
        for (int i = 0; i < cardCount; i++) {
            cardsInHand[i].transform.localScale = Vector3.one;
            cardsInHand[i].transform.position = handOrigin.position;

            float arcDistance = initialArcDistance - (arcDistanceInterval * i);
            float angle = arcDistance / radius + (float)(-Math.PI / 2);
            Vector3 cardPosition = new Vector3(circleCenter.x + radius * (float)Math.Cos(angle),
                                               0.05f + (i * 0.012f),
                                               circleCenter.z + radius * (float)Math.Sin(angle));
            Vector3 normal = (cardPosition - circleCenter).normalized;
            cardsInHand[i].transform.position = cardPosition;
            Quaternion targetRotation = Quaternion.LookRotation(normal);
            cardsInHand[i].transform.rotation = targetRotation;
            cardsInHand[i].transform.Rotate(new Vector3(-90f, 0f, 0f));
        }
    }
}
