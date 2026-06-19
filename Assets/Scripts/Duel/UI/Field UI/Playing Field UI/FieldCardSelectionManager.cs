using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class FieldCardSelectionManager : MonoBehaviour {
    public event EventHandler<FieldCardEventArgs<CreatureFieldCardUI>> OnSelectCreatureFieldCard;
    public event EventHandler<FieldCardEventArgs<CreatureFieldCardUI>> OnSelectCreatureFieldCardDrag;
    public event EventHandler<FieldCardEventArgs<CreatureFieldCardUI>> OnReleaseCreatureFieldCardDrag;
    public event EventHandler<FieldCardEventArgs<CreatureFieldCardUI>> OnReleaseCreatureFieldCardDragFinished;
    public event EventHandler<FieldCardEventArgs<FieldCardUI>> OnInspectFieldCard;

    public static FieldCardSelectionManager Instance {  get; private set; }

    [SerializeField] private float dragOffset;
    private Camera cam;
    private bool isDragging;
    private CreatureFieldCardUI draggingCard;

    private void Awake() {
        if (Instance != null) {
            Debug.LogWarning("FieldCardSelectionManager already exists in scene. Destroying redundant object.");
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start() {
        cam = Camera.main;

        PlayerInputActions playerInputActions = GameInputManager.Instance.PlayerInputActions;
        playerInputActions.Player.Select.started += SelectCreatureFieldCard;
        playerInputActions.Player.Select.canceled += ReleaseCreatureFieldCardDrag;
        playerInputActions.Player.Inspect.started += InspectFieldCard;
    }

    private void OnDestroy() {
        PlayerInputActions playerInputActions = GameInputManager.Instance.PlayerInputActions;
        playerInputActions.Player.Select.started -= SelectCreatureFieldCard;
        playerInputActions.Player.Select.canceled -= ReleaseCreatureFieldCardDrag;
        playerInputActions.Player.Inspect.started -= InspectFieldCard;
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

    private void SelectCreatureFieldCard(InputAction.CallbackContext context) {
        if (!context.started)
            return;
        CreatureFieldCardUI cardUI = CreatureFieldCardRaycastColliderCheck();
        if (cardUI == null)
            return;
        if (!cardUI.IsSelectable)
            return;

        FieldCardEventArgs<CreatureFieldCardUI> args = new FieldCardEventArgs<CreatureFieldCardUI>(cardUI);
        OnSelectCreatureFieldCard?.Invoke(this, args);
        if (args.IsCanceled)
            return;

        OnSelectCreatureFieldCardDrag?.Invoke(this, new FieldCardEventArgs<CreatureFieldCardUI>(cardUI));
        isDragging = true;
        draggingCard = cardUI;
        draggingCard.transform.position = new Vector3(draggingCard.transform.position.x,
                                                      draggingCard.transform.position.y + dragOffset,
                                                      draggingCard.transform.position.z);
    }

    private void ReleaseCreatureFieldCardDrag(InputAction.CallbackContext context) {
        if (!context.canceled)
            return;
        if (!isDragging)
            return;

        CreatureFieldCardUI cardUI = draggingCard;
        ResetCardDragging();
        OnReleaseCreatureFieldCardDrag?.Invoke(this, new FieldCardEventArgs<CreatureFieldCardUI>(cardUI));
        OnReleaseCreatureFieldCardDragFinished?.Invoke(this, new FieldCardEventArgs<CreatureFieldCardUI>(cardUI));
    }

    private void InspectFieldCard(InputAction.CallbackContext context) {
        if (!context.started)
            return;
        FieldCardUI cardUI = FieldCardRaycastColliderCheck();
        if (cardUI == null)
            return;

        // TODO: Implement inspecting cards. Could implemnt a CardUI interface that is implemented
        // by hand and field cards
    }

    private FieldCardUI FieldCardRaycastColliderCheck() {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);
        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        FieldCardUI cardUI = null;
        foreach (RaycastHit hit in hits) {
            FieldCardCollisionPointer fieldCardCollisionPointer;
            if (hit.collider.TryGetComponent<FieldCardCollisionPointer>(out fieldCardCollisionPointer)) {
                cardUI = fieldCardCollisionPointer.GetFieldCardUI();
                break;
            }
        }

        return cardUI;
    }

    private CreatureFieldCardUI CreatureFieldCardRaycastColliderCheck() {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);
        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        CreatureFieldCardUI cardUI = null;
        foreach (RaycastHit hit in hits) {
            if (hit.collider.GetComponent<CreatureFieldCardCollisionPointer>()) {
                cardUI = hit.collider.GetComponent<CreatureFieldCardCollisionPointer>().CardUI;
                break;
            }
        }

        return cardUI;
    }

    public void ResetCardDragging() {
        isDragging = false;
        draggingCard = null;
    }
}