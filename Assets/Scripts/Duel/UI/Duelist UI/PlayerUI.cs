using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUI : DuelistUI {
    public event EventHandler<HandCardDragEventArgs> OnSelectingCardDrag;

    [SerializeField] private Vector3 handHoverOffset;
    [SerializeField] private Vector3 cardHoverOffset;
    [SerializeField] private float cardHoverScale;

    private Camera cam;
    private HandCardUI previousSelection;
    private bool isDragging;
    private HandCardUI draggingCard;

    private void Awake() {
        previousSelection = null;
        isDragging = false;
        draggingCard = null;
    }

    private void Start() {
        cam = Camera.main;

        PlayerInputActions playerInputActions = GameInputManager.Instance.PlayerInputActions;
        playerInputActions.Player.Select.started += SelectCardDrag;
        playerInputActions.Player.Select.canceled += ReleaseCardDrag;
    }

    private void OnDestroy() {
        PlayerInputActions playerInputActions = GameInputManager.Instance.PlayerInputActions;
        playerInputActions.Player.Select.started -= SelectCardDrag;
        playerInputActions.Player.Select.canceled -= ReleaseCardDrag;
    }

    private void Update() {
        UpdateDragging();
        UpdateHovering();
    }

    public void UpdateDragging() {
        if (!isDragging)
            return;
        if (draggingCard == null)
            throw new Exception("Dragging card is null while isDragging is true");

        Vector3 dragPosition = GetScreenToWorldSapceVector();
        draggingCard.transform.position = new Vector3(dragPosition.x, draggingCard.transform.position.y, dragPosition.z);
    }

    public void UpdateHovering() {
        if (isDragging)
            return;

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

    private Vector3 GetScreenToWorldSapceVector() {
        float endPoint = draggingCard.transform.position.y;
        Vector3 origin = cam.transform.position;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        float t = (endPoint - origin.y) / ray.direction.y;

        return ray.direction * t + origin;
    }

    public override void DrawCard(Card card) {
        if(card is CreatureCard creatureCard) {
            CreatureHandCardUI cardUI = Instantiate(creatureCardPrefab, handOrigin);
            cardUI.Init(creatureCard);
            cardUI.transform.Rotate(90f, 0, 0);
            cardsInHand.Add(cardUI);
        }
        else if (card is SpellCard spellCard) {
            SpellHandCardUI cardUI = Instantiate(spellCardPrefab, handOrigin);
            cardUI.Init(spellCard);
            cardUI.transform.Rotate(90f, 0, 0);
            cardsInHand.Add(cardUI);
        }
        else {
            NullHandCardUI cardUI = Instantiate(nullCardPrefab, handOrigin);
            cardUI.transform.Rotate(90f, 0, 0);
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

    public void SetCardSelectable(Guid cardUuid) {
        if (!ContainsCard(cardUuid))
            throw new Exception("Attempting to set selectable card border visibility to card that is not in the PlayUI hand");

        GetCardByUuid(cardUuid).SetBorderVisibility(true);
    }

    public void SetBorderVisibilityAll(bool isVisiable) {
        foreach(HandCardUI cardUI in cardsInHand)
            cardUI.SetBorderVisibility(isVisiable);
    }

    public void HoverHand() {
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

    public void SelectCardDrag(InputAction.CallbackContext context) {
        if (!context.started)
            return;
        if (isDragging)
            return;
        HandCardUI cardUI = HandCardRaycast();
        if (cardUI == null)
            return;
        if (!ContainsCard(cardUI))
            throw new Exception("Unable to find handCardUI for SelectCardDrag");

        int handCardIndex = IndexOf(cardUI);
        HandCardDragEventArgs args = new HandCardDragEventArgs(this, cardUI, handCardIndex);
        OnSelectingCardDrag?.Invoke(this, args);
        if (args.IsCancelled)
            return;

        EventBus.InvokeOnStartHandCardDrag(this, new HandCardDragEventArgs(this, cardUI, handCardIndex));
        isDragging = true;
        draggingCard = cardUI;
        draggingCard.transform.localScale = Vector3.one;
        draggingCard.transform.eulerAngles = new Vector3(draggingCard.transform.eulerAngles.x, 0f, draggingCard.transform.eulerAngles.z);
        draggingCard.transform.position = new Vector3(draggingCard.transform.position.x, handOrigin.transform.position.y, draggingCard.transform.position.z);
        SetDefaultCardPositions();
    }

    public void ReleaseCardDrag(InputAction.CallbackContext context) {
        if (!context.canceled)
            return;
        if (!isDragging)
            return;
        if (!ContainsCard(draggingCard))
            throw new Exception("Unable to find draggingCard for ReleaseCardDrag");

        HandCardUI cardUI = draggingCard;
        ResetCardDragging();
        EventBus.InvokeOnReleaseHandCardDrag(this, new HandCardDragEventArgs(this, cardUI, IndexOf(cardUI)));
    }

    private HandCardUI HandCardRaycast() {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);
        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        HandCardUI cardUI = null;
        foreach (RaycastHit hit in hits) {
            if (hit.collider.GetComponent<HandCardCollisionPointer>()) {
                cardUI = hit.collider.GetComponent<HandCardCollisionPointer>().HandCardUI;
                break;
            }
        }

        return cardUI;
    }

    public void ResetCardDragging() {
        isDragging = false;
        draggingCard = null;
        SetDefaultCardPositions();
    }

    public override void SetDefaultCardPositions() {
        float cardSpacing = 0.34f;
        float cardVerticalOffset = -0.003f;
        float cardRotation = 1f;

        int cardCount = cardsInHand.Count;
        float handOffsetX = (cardCount - 1) * cardSpacing / 2;
        float centerCardPoint = (cardCount - 1) / 2f;
        float handRotation = (cardCount - 1) * cardRotation / 2;
        for (int i = 0; i < cardCount; i++) {
            if(isDragging && i == GetDraggingCardIndex())
                continue;

            cardsInHand[i].transform.localScale = Vector3.one;
            cardsInHand[i].transform.position = handOrigin.position;
            float xOffset = (i * cardSpacing - handOffsetX);
            float zOffset = (float)Math.Floor(Math.Abs(i - centerCardPoint)) * cardVerticalOffset * 3f;
            Vector3 cardPosition = new Vector3(xOffset, i * 0.005f, zOffset);
            cardsInHand[i].transform.Translate(cardPosition, Space.World);
            cardsInHand[i].transform.eulerAngles = new Vector3(cardsInHand[i].transform.eulerAngles.x, 0f, cardsInHand[i].transform.eulerAngles.z);
            cardsInHand[i].transform.Rotate(new Vector3(0, i * cardRotation - handRotation, 0), Space.World);
        }
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

    public int GetDraggingCardIndex() {
        if (!isDragging)
            throw new Exception("Card is not currently being dragged");

        for(int i = 0; i < cardsInHand.Count; i++) {
            if (cardsInHand[i] == draggingCard)
                return i;
        }
        throw new Exception("Unable to find dragging card");
    }

    public HandCardUI GetCardByUuid(Guid cardUuid) {
        foreach(HandCardUI cardUI in cardsInHand) {
            if(cardUI.CardUuid == cardUuid)
                return cardUI;
        }
        throw new Exception("Attempted to get cardUI that does not exists in PlayerUI hand");
    }

    public bool IsDragging { get { return isDragging; } }

    public HandCardUI DraggingCard { get { return draggingCard; } }
}
