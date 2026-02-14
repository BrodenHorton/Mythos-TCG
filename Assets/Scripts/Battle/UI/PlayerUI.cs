using System;
using UnityEngine;

public class PlayerUI : ResourceUI {
    [SerializeField] private Vector3 handInspectOffset;
    [SerializeField] private bool isInspectActive;

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

        isInspectActive = false;
    }

    public void DrawCard(object sender, DrawCardEventArgs args) {
        if (playerUuid != args.Player.Uuid)
            return;

        HandCardUI drawnCard = Instantiate(card, handOrigin);
        drawnCard.transform.Rotate(90f, 0, 0);
        cardsInHand.Add(drawnCard);
        drawnCard.OnSelected += PlayCardFromHand;
        SpaceCards();
    }

    private void SetSelectableCards(object sender, EventArgs args) {
        DuelManager duelManager = FindFirstObjectByType<DuelManager>();
        if (playerUuid != duelManager.GetCurrentPlayerTurn().Uuid)
            return;

        MatchPlayer player = duelManager.GetPlayerByUuid(playerUuid);
        for (int i = 0; i < cardsInHand.Count; i++) {
            if (player.Hand.Count <= i)
                break;

            Debug.Log("Setting Selectable Card for Player: " + player.Uuid);
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
        SpaceCards();
        duelManager.PlayCardInHand(playerUuid, cardIndex);
    }

    public void InspectHand() {
        if (isInspectActive)
            return;

        for (int i = 0; i < cardsInHand.Count; i++)
            cardsInHand[i].transform.Translate(handInspectOffset, Space.World);
        isInspectActive = true;
    }
}
