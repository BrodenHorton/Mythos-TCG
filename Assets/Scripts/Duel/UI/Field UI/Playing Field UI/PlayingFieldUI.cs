using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayingFieldUI : MonoBehaviour {
    public event EventHandler<SelectFieldCardDragEventArgs> OnSelectCardDrag;
    public event EventHandler<ReleaseFieldCardDragEventArgs> OnReleaseCardDrag;

    [SerializeField] private Transform creatureSlotOrigin;
    [SerializeField] private Transform domainSlotOrigin;
    [SerializeField] private List<CreatureFieldCardUI> creatures;
    [SerializeField] private SpellFieldCardUI domainCard;
    [Header("Prefabs")]
    [SerializeField] private CreatureFieldCardUI creatureCardUIPrefab;
    [SerializeField] private SpellFieldCardUI spellCardUIPrefab;

    private ulong playerId;
    private float cardSpacing = 0.6f;
    private Camera cam;
    private bool isDragging;
    private CreatureFieldCardUI draggingCard;

    private void Awake() {
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
    }

    public void Init(ulong playerId) {
        this.playerId = playerId;
    }

    public void UpdateDragging() {
        if (!isDragging)
            return;
        if (draggingCard == null)
            throw new Exception("Dragging card is null while isDragging is true");

        Vector3 dragPosition = GetScreenToWorldSapceVector();
        draggingCard.transform.position = new Vector3(dragPosition.x, draggingCard.transform.position.y, dragPosition.z);
    }

    private Vector3 GetScreenToWorldSapceVector() {
        float endPoint = draggingCard.transform.position.y;
        Vector3 origin = cam.transform.position;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        float t = (endPoint - origin.y) / ray.direction.y;

        return ray.direction * t + origin;
    }

    public void PlayCreatureCard(CreatureCard card) {
        CreatureFieldCardUI creatureCardUI = Instantiate(creatureCardUIPrefab);
        creatureCardUI.transform.parent = creatureSlotOrigin;
        creatureCardUI.Init(card);
        creatures.Add(creatureCardUI);
        SetDefaultCardPositions();
    }

    // TODO: When Adding a field card, make sure to pass the Match Player so you can sort the field cards to match
    // the players internal creature order
    public void AddCreatureFieldCard(CreatureFieldCardUI cardUI) {
        creatures.Add(cardUI);
        SetDefaultCardPositions();
    }

    public void PlayDomainCard(SpellCard card) {
        SpellFieldCardUI domainCardUI = Instantiate(spellCardUIPrefab, domainSlotOrigin);
        domainCardUI.Init(card);
        domainCard = domainCardUI;
    }

    public void TapCreature(CreatureCard card) {
        foreach (CreatureFieldCardUI cardUI in creatures) {
            if (cardUI.CardUuid == card.Uuid) {
                cardUI.Tap();
                break;
            }
        }
    }

    public void UntapCreature(CreatureCard card) {
        foreach(CreatureFieldCardUI cardUI in creatures) {
            if(cardUI.CardUuid == card.Uuid) {
                cardUI.Untap();
                break;
            }
        }
    }

    public void RemoveCreature(Guid uuid) {
        foreach(CreatureFieldCardUI cardUI in creatures) {
            if(cardUI.CardUuid == uuid) {
                RemoveCreature(cardUI);
                break;
            }
        }
    }

    public void RemoveCreature(CreatureFieldCardUI cardUI) {
        creatures.Remove(cardUI);
        Destroy(cardUI.gameObject);
        SetDefaultCardPositions();
    }

    private void SelectCardDrag(InputAction.CallbackContext context) {
        if (!context.started)
            return;
        CreatureFieldCardUI cardUI = CreatureFieldCardRaycastColliderCheck();
        if (cardUI == null)
            return;
        if (!ContainsCreature(cardUI))
            return;

        SelectFieldCardDragEventArgs args = new SelectFieldCardDragEventArgs(this, cardUI);
        OnSelectCardDrag?.Invoke(this, args);
        if (args.IsCancelled)
            return;

        isDragging = true;
        draggingCard = cardUI;
        float dragOffset = 1f;
        draggingCard.transform.position = new Vector3(draggingCard.transform.position.x, draggingCard.transform.position.y + dragOffset, draggingCard.transform.position.z);
    }

    private void ReleaseCardDrag(InputAction.CallbackContext context) {
        if (!context.canceled)
            return;
        if (!isDragging)
            return;
        if (!ContainsCreature(draggingCard))
            throw new Exception("Unable to find draggingCard for ReleaseCardDrag");

        bool isReleasedInCombatArea = IsHoveringCombatArea();
        CreatureFieldCardUI cardUI = draggingCard;
        ResetCardDragging();
        OnReleaseCardDrag?.Invoke(this, new ReleaseFieldCardDragEventArgs(this, cardUI, isReleasedInCombatArea));
    }

    private CreatureFieldCardUI CreatureFieldCardRaycastColliderCheck() {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);
        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        CreatureFieldCardUI fieldCardUI = null;
        foreach (RaycastHit hit in hits) {
            if (hit.collider.GetComponent<CreatureFieldCardCollisionPointer>()) {
                fieldCardUI = hit.collider.GetComponent<CreatureFieldCardCollisionPointer>().FieldCardUI;
                break;
            }
        }

        return fieldCardUI;
    }

    // TODO: Create combat field areas as indicators
    private bool IsHoveringCombatArea() {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);
        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        foreach (RaycastHit hit in hits) {
            if (hit.collider.GetComponent<PlayerUIPlayableAreaIndicator>() != null)
                return true;
        }

        return false;
    }

    public void ResetCardDragging() {
        isDragging = false;
        draggingCard = null;
        SetDefaultCardPositions();
    }

    public void SetSelectableCards(MatchPlayer player) {
        for (int i = 0; i < creatures.Count; i++) {
            if (player.Creatures.Count <= i)
                throw new Exception("Creature cards in model and view do not match");

            creatures[i].SetBorderVisibility(player.Creatures[i].CanAttack());
        }
    }

    public bool ContainsCreature(CreatureFieldCardUI other) {
        foreach (CreatureFieldCardUI cardUI in creatures) {
            if (cardUI == other)
                return true;
        }

        return false;
    }

    public bool ContainsCreature(Guid cardUuid) {
        foreach (CreatureFieldCardUI cardUI in creatures) {
            if (cardUI.CardUuid == cardUuid)
                return true;
        }

        return false;
    }

    private void SetDefaultCardPositions() {
        int cardCount = creatures.Count;
        float handOffset = (cardCount - 1) * cardSpacing / 2;
        for (int i = 0; i < cardCount; i++) {
            FieldCardUI cardUI = creatures[i];
            cardUI.transform.localScale = Vector3.one;
            cardUI.transform.eulerAngles = Vector3.zero;
            Vector3 cardPosition = creatureSlotOrigin.position;
            cardPosition.x += i * cardSpacing - handOffset;
            cardUI.transform.position = cardPosition;
        }
    }

    public CreatureFieldCardUI GetCreatureFieldCardUIBy(Guid uuid) {
        foreach(CreatureFieldCardUI cardUI in creatures) {
            if(cardUI.CardUuid == uuid)
                return cardUI;
        }

        return null;
    }

    public ulong PlayerId { get { return playerId; } }
}
