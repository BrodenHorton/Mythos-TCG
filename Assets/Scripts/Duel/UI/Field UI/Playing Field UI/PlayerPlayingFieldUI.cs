using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPlayingFieldUI : PlayingFieldUI {
    public event EventHandler<FieldCardDragEventArgs<CreatureFieldCardUI>> OnSelectingCardDrag;

    [Header("Card Drag")]
    [SerializeField] private float dragOffset;

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

    private void SelectCardDrag(InputAction.CallbackContext context) {
        if (!context.started)
            return;
        CreatureFieldCardUI cardUI = CreatureFieldCardRaycastColliderCheck();
        if (cardUI == null)
            return;
        if (!ContainsCreature(cardUI))
            return;

        FieldCardDragEventArgs<CreatureFieldCardUI> args = new FieldCardDragEventArgs<CreatureFieldCardUI>(this, cardUI);
        OnSelectingCardDrag?.Invoke(this, args);
        if (args.IsCancelled)
            return;

        EventBus.InvokeOnStartCardDragPlayingField(this, args);
        isDragging = true;
        draggingCard = cardUI;
        draggingCard.transform.position = new Vector3(draggingCard.transform.position.x, draggingCard.transform.position.y + dragOffset, draggingCard.transform.position.z);
    }

    private void ReleaseCardDrag(InputAction.CallbackContext context) {
        if (!context.canceled)
            return;
        if (!isDragging)
            return;
        if (!ContainsCreature(draggingCard))
            throw new Exception("Unable to find draggingCard for ReleaseCardDrag");

        CreatureFieldCardUI cardUI = draggingCard;
        ResetCardDragging();
        EventBus.InvokeOnReleaseCardDragPlayingField(this, new ReleaseFieldCardDragEventArgs<CreatureFieldCardUI>(this, cardUI));
    }

    private CreatureFieldCardUI CreatureFieldCardRaycastColliderCheck() {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);
        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        CreatureFieldCardUI fieldCardUI = null;
        foreach (RaycastHit hit in hits) {
            if (hit.collider.GetComponent<CreatureFieldCardCollisionPointer>()) {
                fieldCardUI = hit.collider.GetComponent<CreatureFieldCardCollisionPointer>().CardUI;
                break;
            }
        }

        return fieldCardUI;
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
}
