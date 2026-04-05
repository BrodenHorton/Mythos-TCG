using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayingFieldUI : MonoBehaviour {
    public event EventHandler<CreatureFieldCardDragEventArgs> OnSelectingCardDrag;

    [SerializeField] private Transform creatureSlotOrigin;
    [SerializeField] private Transform domainSlotOrigin;
    [SerializeField] private float cardSpacing;
    [SerializeField] private float dragOffset;
    [Header("Field Cards")]
    [SerializeField] private List<CreatureFieldCardUI> creatures;
    [SerializeField] private SpellFieldCardUI domainCard;
    [Header("Prefabs")]
    [SerializeField] private CreatureFieldCardUI creatureCardUIPrefab;
    [SerializeField] private SpellFieldCardUI spellCardUIPrefab;

    private ulong playerId;
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

    public void PlayCreatureCard(MatchPlayer player, CreatureCard card) {
        CreatureFieldCardUI creatureCardUI = Instantiate(creatureCardUIPrefab);
        creatureCardUI.transform.parent = creatureSlotOrigin;
        creatureCardUI.Init(card);
        AddCreatureFieldCard(player, creatureCardUI);
    }

    public void AddCreatureFieldCard(MatchPlayer player, CreatureFieldCardUI cardUI) {
        creatures.Add(cardUI);
        SortCreatures(player);
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

        CreatureFieldCardDragEventArgs args = new CreatureFieldCardDragEventArgs(this, cardUI);
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
        EventBus.InvokeOnReleaseCardDragPlayingField(this, new ReleaseCreatureFieldCardDragEventArgs(this, cardUI));
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

    private void SortCreatures(MatchPlayer player) {
        List<CreatureFieldCardUI> sortedList = new List<CreatureFieldCardUI>();
        for(int i = 0; i < player.Creatures.Count; i++) {
            CreatureFieldCardUI cardUI = GetCreatureFieldCardUIBy(player.Creatures[i].Uuid);
            if (cardUI == null)
                continue;

            sortedList.Add(cardUI);
        }
        creatures = sortedList;
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
