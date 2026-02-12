using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour {
    [SerializeField] private Transform deckOrigin;
    [SerializeField] private Transform handOrigin;
    [SerializeField] private List<HandCardUI> cardsInHand;
    [Header("Prefabs")]
    [SerializeField] private HandCardUI card;

    private Guid playerUuid;
    private float cardSpacing = 0.55f;

    private void Start() {
        DuelManager duelManager = FindFirstObjectByType<DuelManager>();
        if (duelManager == null)
            throw new Exception("Could not find DuelManager object");
        DuelStateManager stateManager = FindFirstObjectByType<DuelStateManager>();
        if (stateManager == null)
            throw new Exception("Could not find DuelStateManager object");

        duelManager.OnDrawCard += DrawCard;

        stateManager.DrawPhase.OnDrawPhase += (sender, e) => {
            for (int i = 0; i < cardsInHand.Count; i++) {
                cardsInHand[i].SetBorderVisibility(false);
            }
        };

        MatchPlayer player = duelManager.GetPlayerByUuid(playerUuid);
        stateManager.StartPhase.OnStartPhase += (sender, e) => {
            for(int i = 0; i < cardsInHand.Count; i++) {
                if (player.Hand.Count <= i)
                    break;

                cardsInHand[i].SetBorderVisibility(player.Hand[i].IsPlayable());
            }
        };
    }

    public void DrawCard(object sender, DrawCardEventArgs args) {
        if (playerUuid != args.Player.Uuid)
            return;

        HandCardUI drawnCard = Instantiate(card, handOrigin);
        drawnCard.transform.Rotate(90f, 0, 0);
        cardsInHand.Add(drawnCard);
        SpaceCards();
    }

    private void SpaceCards() {
        int cardCount = cardsInHand.Count;
        float handOffset = (cardCount - 1) * cardSpacing / 2;
        for (int i = 0; i < cardCount; i++) {
            Vector3 cardPosition = handOrigin.position;
            cardPosition.x += i * cardSpacing - handOffset;
            cardsInHand[i].transform.position = cardPosition;
        }
    }

    public Guid PlayerUuid { get { return playerUuid; } set { playerUuid = value; } }
}