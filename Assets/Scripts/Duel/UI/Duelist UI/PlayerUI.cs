using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : DuelistUI {
    [SerializeField] private Collider playableAreaCollider;
    [SerializeField] private GameObject playableAreaVisual;
    [SerializeField] private Vector3 handHoverOffset;
    [SerializeField] private Vector3 cardHoverOffset;
    [SerializeField] private float cardHoverScale;

    private Camera cam;
    private HandCardUI previousSelection;
    private Vector3 handCircleCenter;

    private void Awake() {
        previousSelection = null;
        handCircleCenter = new Vector3(handOrigin.position.x,
                                           handOrigin.position.y,
                                           handOrigin.position.z - radius);
    }

    private void Start() {
        cam = Camera.main;

        playableAreaVisual.SetActive(false);
    }

    public void UpdateHovering() {
        HandCardUI cardUI = HoverDetection();
        if (cardUI == null && previousSelection != null) {
            SetDefaultCardPositions();
            previousSelection = null;
        }
        else if (cardUI != null && ContainsCard(cardUI)) {
            if (previousSelection == null) {
                HoverHand();
                HoverCard(cardUI);
                previousSelection = cardUI;
            }
            else if (cardUI != previousSelection) {
                ExitHoverCard(previousSelection);
                HoverCard(cardUI);
                previousSelection = cardUI;
            }
        }
    }

    public override void DrawCard(CardPayload card) {
        if(card is CreatureCardPayload creatureCard) {
            CreatureHandCardUI cardUI = Instantiate(creatureCardPrefab, handOrigin);
            cardUI.Init(creatureCard, playerId);
            cardUI.transform.Rotate(90f, 0, 0);
            cardsInHand.Add(cardUI);
        }
        else if (card is SpellCardPayload spellCard) {
            SpellHandCardUI cardUI = Instantiate(spellCardPrefab, handOrigin);
            cardUI.Init(spellCard, playerId);
            cardUI.transform.Rotate(90f, 0, 0);
            cardsInHand.Add(cardUI);
        }
        else if (card is DomainCardPayload domainCard) {
            DomainHandCardUI cardUI = Instantiate(domainCardPrefab, handOrigin);
            cardUI.Init(domainCard, playerId);
            cardUI.transform.Rotate(90f, 0, 0);
            cardsInHand.Add(cardUI);
        }
        else {
            NullHandCardUI cardUI = Instantiate(nullCardPrefab, handOrigin);
            cardUI.Init(playerId);
            cardUI.transform.Rotate(90f, 0, 0);
            cardsInHand.Add(cardUI);
        }

        SetDefaultCardPositions();
    }

    public override void RemoveCardFromHand(Guid cardUuid) {
        if (!ContainsCard(cardUuid))
            throw new Exception("Attempting to remove card that is not in player's hand. Card uuid: " + cardUuid);

        HandCardUI cardUI = GetCardByUuid(cardUuid);
        cardsInHand.Remove(cardUI);
        cardUI.RemoveListeners();
        Destroy(cardUI.gameObject);
        SetDefaultCardPositions();
    }

    public void HoverHand() {
        for (int i = 0; i < cardsInHand.Count; i++)
            cardsInHand[i].transform.Translate(handHoverOffset, Space.World);
    }

    public void HoverCard(HandCardUI card) {
        card.transform.Translate(cardHoverOffset, Space.World);
        card.transform.localScale = new Vector3(cardHoverScale, cardHoverScale, cardHoverScale);
        card.transform.localEulerAngles = new Vector3(card.transform.localEulerAngles.x,
                                                      0f,
                                                      card.transform.localEulerAngles.z);
    }

    public void ExitHoverCard(HandCardUI cardUI) {
        cardUI.transform.Translate(-cardHoverOffset, Space.World);
        cardUI.transform.localScale = new Vector3(1f, 1f, 1f);
        ResetHandCardRotation(IndexOf(cardUI.CardUuid));
    }

    public override void SetDefaultCardPositions() {
        SetDefaultCardPositions(new List<Guid>());
    }

    public override void SetDefaultCardPositions(List<Guid> ignoreCards) {
        // TODO: Figure out how to detect which axis and direction the radius should be added to so you get the correct circle center
        int cardCount = cardsInHand.Count;
        float initialArcDistance = (cardCount - 1) * arcDistanceInterval / 2;
        for (int i = 0; i < cardCount; i++) {
            cardsInHand[i].transform.localScale = Vector3.one;

            if (ignoreCards.Contains(cardsInHand[i].CardUuid))
                continue;

            cardsInHand[i].transform.position = handOrigin.position;

            float arcDistance = initialArcDistance - (arcDistanceInterval * i);
            float angle = arcDistance / radius + (float)(Math.PI / 2);
            Vector3 cardPosition = new Vector3(handCircleCenter.x + radius * (float)Math.Cos(angle),
                                               0.05f + (i * 0.012f),
                                               handCircleCenter.z + radius * (float)Math.Sin(angle));
            Vector3 normal = (cardPosition - handCircleCenter).normalized;
            cardsInHand[i].transform.position = cardPosition;
            Quaternion targetRotation = Quaternion.LookRotation(normal);
            cardsInHand[i].transform.rotation = targetRotation;
            cardsInHand[i].transform.Rotate(new Vector3(90f, 0f, 0f));
        }
    }

    private void ResetHandCardRotation(int cardIndex) {
        if (cardIndex >= cardsInHand.Count)
            throw new Exception("Attempting to set the card rotation of an out of bounds card index.Cards in hand: " + cardsInHand + " Card Index: " + cardIndex);

        // TODO: Figure out how to detect which axis and direction the radius should be added to so you get the correct circle center
        int cardCount = cardsInHand.Count;
        float initialArcDistance = (cardCount - 1) * arcDistanceInterval / 2;
        float arcDistance = initialArcDistance - (arcDistanceInterval * cardIndex);
        float angle = arcDistance / radius + (float)(Math.PI / 2);
        Vector3 cardPosition = new Vector3(handCircleCenter.x + radius * (float)Math.Cos(angle),
                                           0.05f + (cardIndex * 0.012f),
                                           handCircleCenter.z + radius * (float)Math.Sin(angle));
        Vector3 normal = (cardPosition - handCircleCenter).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(normal);
        cardsInHand[cardIndex].transform.rotation = targetRotation;
        cardsInHand[cardIndex].transform.Rotate(new Vector3(90f, 0f, 0f));
    }

    private HandCardUI HoverDetection() {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);
        if (hits.Length > 0)
            Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        foreach (RaycastHit hit in hits) {
            if (hit.collider.GetComponent<HandCardCollisionPointer>())
                return hit.collider.GetComponent<HandCardCollisionPointer>().HandCardUI;
        }

        return null;
    }

    public void ShowPlayableAreaVisual() {
        playableAreaVisual.SetActive(true);
    }

    public void HidePlayableAreaVisual() {
        playableAreaVisual.SetActive(false);
    }

    public bool IsHoveringPlayableArea() {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);
        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        foreach (RaycastHit hit in hits) {
            if (hit.collider == playableAreaCollider)
                return true;
        }

        return false;
    }

    public int IndexOf(Guid cardUuid) {
        for (int i = 0; i < cardsInHand.Count; i++) {
            if (cardsInHand[i].CardUuid == cardUuid)
                return i;
        }
        throw new Exception("Unable for find card with uuid: " +  cardUuid);
    }

    public HandCardUI GetCardByUuid(Guid cardUuid) {
        foreach(HandCardUI cardUI in cardsInHand) {
            if(cardUI.CardUuid == cardUuid)
                return cardUI;
        }
        throw new Exception("Attempted to get cardUI that does not exists in PlayerUI hand");
    }
}
