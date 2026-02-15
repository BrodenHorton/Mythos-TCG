using System;
using UnityEngine;

public class PlayerUI : ResourceUI {
    [SerializeField] private Vector3 handHoverOffset;
    [SerializeField] private Vector3 cardHoverOffset;
    [SerializeField] private float cardHoverScale;

    public void DrawCard() {
        Debug.Log("Drawing Card");
        HandCardUI drawnCard = Instantiate(card, handOrigin);
        drawnCard.transform.Rotate(90f, 0, 0);
        cardsInHand.Add(drawnCard);
        drawnCard.OnSelected += PlayCardFromHand;
        DefaultCardPositions();
    }

    public void SetManaCount(int manaCount) {
        this.manaCount.text = manaCount.ToString();
    }

    public void SetSelectableCards() {
        DuelManager duelManager = FindFirstObjectByType<DuelManager>();
        if (playerUuid != duelManager.GetCurrentPlayerTurn().Uuid)
            return;

        MatchPlayer player = duelManager.GetPlayerByUuid(playerUuid);
        for (int i = 0; i < cardsInHand.Count; i++) {
            if (player.Hand.Count <= i)
                break;

            cardsInHand[i].SetBorderVisibility(player.Hand[i].IsPlayable(duelManager, player));
        }
    }

    private void PlayCardFromHand(object sender, HandCardSelectedEventArgs args) {
        int cardIndex = IndexOf(args.CardUI);
        if (cardIndex == -1)
            return;

        cardsInHand.RemoveAt(cardIndex);
        Destroy(args.CardUI.gameObject);
        DefaultCardPositions();
    }

    public int IndexOf(HandCardUI cardUI) {
        int cardIndex = -1;
        for (int i = 0; i < cardsInHand.Count; i++) {
            if (cardsInHand[i].Equals(cardUI)) {
                cardIndex = i;
                break;
            }
        }

        return cardIndex;
    }

    public void DefaultCardPositions() {
        float cardSpacing = 0.34f;
        float cardVerticalOffset = -0.003f;
        float cardRotation = 1f;

        int cardCount = cardsInHand.Count;
        float handOffsetX = (cardCount - 1) * cardSpacing / 2;
        float centerCardPoint = (cardCount - 1) / 2f;
        float handRotation = (cardCount - 1) * cardRotation / 2;
        for (int i = 0; i < cardCount; i++) {
            cardsInHand[i].transform.localScale = new Vector3(1f, 1f, 1f);
            cardsInHand[i].transform.position = handOrigin.position;
            float xOffset = (i * cardSpacing - handOffsetX);
            float zOffset = (float)Math.Floor(Math.Abs(i - centerCardPoint)) * cardVerticalOffset * 3f;
            Vector3 cardPosition = new Vector3(xOffset, i * 0.005f, zOffset);
            cardsInHand[i].transform.Translate(cardPosition, Space.World);
            cardsInHand[i].transform.eulerAngles = new Vector3(cardsInHand[i].transform.eulerAngles.x, 0f, cardsInHand[i].transform.eulerAngles.z);
            cardsInHand[i].transform.Rotate(new Vector3(0, i * cardRotation - handRotation, 0), Space.World);
        }
    }

    public void InspectHand() {
        for (int i = 0; i < cardsInHand.Count; i++)
            cardsInHand[i].transform.Translate(handHoverOffset, Space.World);
    }

    public void HoverCard(HandCardUI card) {
        card.transform.Translate(cardHoverOffset, Space.World);
        card.transform.localScale = new Vector3(cardHoverScale, cardHoverScale, cardHoverScale);
    }

    public void ExitHoverCard(HandCardUI card) {
        card.transform.Translate(-cardHoverOffset, Space.World);
        card.transform.localScale = new Vector3(1f, 1f, 1f);
    }
}
