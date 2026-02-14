using System;
using UnityEngine;

public class PlayerUI : ResourceUI {
    [SerializeField] private Vector3 handInspectOffset;

    private void Start() {
        DuelManager duelManager = FindFirstObjectByType<DuelManager>();
        if (duelManager == null)
            throw new Exception("Could not find DuelManager object");
        DuelStateManager stateManager = FindFirstObjectByType<DuelStateManager>();
        if (stateManager == null)
            throw new Exception("Could not find DuelStateManager object");

        duelManager.OnManaCountChanged += (sender, e) => {
            if(e.Player.Uuid == playerUuid)
                manaCount.text = e.CurrentMana.ToString();
        };

        duelManager.OnDrawCard += DrawCard;

        stateManager.DrawPhase.OnDrawPhase += (sender, e) => {
            for (int i = 0; i < cardsInHand.Count; i++)
                cardsInHand[i].SetBorderVisibility(false);
        };

        stateManager.StartPhase.OnStartPhase += SetSelectableCards;
    }

    public void DrawCard(object sender, DrawCardEventArgs args) {
        Debug.Log("Drawing Card before check in PlayerUI. Uuid: " + args.Player.Uuid);
        if (playerUuid != args.Player.Uuid)
            return;

        Debug.Log("Drawing Card");
        HandCardUI drawnCard = Instantiate(card, handOrigin);
        drawnCard.transform.Rotate(90f, 0, 0);
        cardsInHand.Add(drawnCard);
        drawnCard.OnSelected += PlayCardFromHand;
        DefaultCardPositions();
    }

    private void SetSelectableCards(object sender, EventArgs args) {
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
        DuelManager duelManager = FindFirstObjectByType<DuelManager>();
        int cardIndex = -1;
        for(int i = 0; i < cardsInHand.Count; i++) {
            if(cardsInHand[i].Equals(args.CardUI)) {
                cardIndex = i;
                break;
            }
        }
        if (cardIndex == -1)
            throw new Exception("Invalid card selected in hand");

        cardsInHand.RemoveAt(cardIndex);
        Destroy(args.CardUI.gameObject);
        DefaultCardPositions();
        duelManager.PlayCardInHand(playerUuid, cardIndex);
    }

    public void InspectHand() {
        for (int i = 0; i < cardsInHand.Count; i++)
            cardsInHand[i].transform.Translate(handInspectOffset, Space.World);
    }

    public void DefaultCardPositions() {
        float cardSpacing = 0.32f;
        float cardVerticalOffset = -0.003f;
        float cardRotation = 1.6f;

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
