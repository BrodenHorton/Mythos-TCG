using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class ResourceUI : MonoBehaviour {
    [SerializeField] protected Transform deckOrigin;
    [SerializeField] protected Transform handOrigin;
    [SerializeField] protected TextMeshPro manaCount;
    [SerializeField] protected List<HandCardUI> cardsInHand;
    [Header("Prefabs")]
    [SerializeField] protected HandCardUI card;

    protected Guid playerUuid;
    protected float cardSpacing = 0.52f;
    protected float cardVerticalOffset = -0.05f;
    protected float cardRotation = 2f;

    // TODO: Fix card rotation
    protected void SpaceCards() {
        int cardCount = cardsInHand.Count;
        float handOffsetX = (cardCount - 1) * cardSpacing / 2;
        float centerCardPoint = (cardCount - 1) / 2f;
        float handRotation = (cardCount - 1) * cardRotation / 2;
        for (int i = 0; i < cardCount; i++) {
            cardsInHand[i].transform.position = handOrigin.position;
            float xOffset = i * cardSpacing - handOffsetX;
            float zOffset = (float)Math.Floor(Math.Abs(i - centerCardPoint)) * cardVerticalOffset;
            Vector3 cardPosition = new Vector3(xOffset, 0, zOffset);
            cardsInHand[i].transform.Translate(cardPosition, Space.World);
            cardsInHand[i].transform.eulerAngles = new Vector3(cardsInHand[i].transform.eulerAngles.x, 0f, cardsInHand[i].transform.eulerAngles.z);
            cardsInHand[i].transform.Rotate(new Vector3(0, i * cardRotation - handRotation, 0), Space.World);
        }
    }

    public Guid PlayerUuid { get { return playerUuid; } set { playerUuid = value; } }
}
