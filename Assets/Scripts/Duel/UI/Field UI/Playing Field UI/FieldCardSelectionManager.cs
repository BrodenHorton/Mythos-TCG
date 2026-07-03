using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class FieldCardSelectionManager : NetworkBehaviour {
    public event EventHandler<SelectableCardsEventArgs> OnGetSelectableFieldCards;
    public event EventHandler<List<Guid>> OnSetSelectableFieldCards;
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
    private DuelManager duelManager;
    private ActionManager actionManager;
    private CombatManager combatManager;
    private CombatStateManager combatStateManager;

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

        duelManager = ServiceLocator.Get<DuelManager>();
        actionManager = ServiceLocator.Get<ActionManager>();
        combatManager = ServiceLocator.Get<CombatManager>();
        combatStateManager = ServiceLocator.Get<CombatStateManager>();

        combatStateManager.DeclareAttackersState.OnStartDeclareAttackers += (sender, args) => {
            if (!IsServer)
                return;

            SetSelectableCardsForActionFocusPlayers();
        };
        combatStateManager.DeclareDefendersState.OnStartDeclareDefenders += (sender, args) => {
            if (!IsServer)
                return;

            SetSelectableCardsForActionFocusPlayers();
        };
        actionManager.OnActionStateChanged += SetSelectableCardsForActionFocusPlayers;
        EventBus.Instance.OnPostDeclareAttacker += SetSelectableCardsForActionFocusPlayers;
        EventBus.Instance.OnPostDeclareDefender += SetSelectableCardsForActionFocusPlayers;
        EventBus.Instance.OnPostUndeclareAttacker += SetSelectableCardsForActionFocusPlayers;
        EventBus.Instance.OnPostUndeclareDefender += SetSelectableCardsForActionFocusPlayers;

        PlayerInputActions playerInputActions = GameInputManager.Instance.PlayerInputActions;
        playerInputActions.Player.Select.started += SelectCreatureFieldCard;
        playerInputActions.Player.Select.canceled += ReleaseCreatureFieldCardDrag;
        playerInputActions.Player.Inspect.started += InspectFieldCard;
    }

    public override void OnNetworkDespawn() {
        base.OnNetworkDespawn();

        actionManager.OnActionStateChanged -= SetSelectableCardsForActionFocusPlayers;
        EventBus.Instance.OnPostDeclareAttacker -= SetSelectableCardsForActionFocusPlayers;
        EventBus.Instance.OnPostDeclareDefender -= SetSelectableCardsForActionFocusPlayers;
        EventBus.Instance.OnPostUndeclareAttacker -= SetSelectableCardsForActionFocusPlayers;
        EventBus.Instance.OnPostUndeclareDefender -= SetSelectableCardsForActionFocusPlayers;

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

    private void SetSelectableCardsForActionFocusPlayers(object sender, EventArgs args) {
        if (!IsServer)
            return;

        SetSelectableCardsForActionFocusPlayers();
    }

    private void SetSelectableCardsForActionFocusPlayers() {
        if (!IsServer)
            throw new Exception("Only the server can call the method SetSelectableCardsForActionFocusPlayers");

        foreach (ulong playerId in actionManager.ActionFocusPlayerIds)
            SetSelectableCards(playerId);
    }

    private void SetSelectableCards(ulong playerId) {
        if(!IsServer)
            throw new Exception("Only the server can call the method SetSelectableCards");

        FixedString128Bytes[] selectableCardUuidStrs;
        if (actionManager.ActionFocusPlayerIds.Contains(playerId)) {
            List<Guid> selectableCardGuids = GetSelectableCardGuids(playerId);
            selectableCardUuidStrs = new FixedString128Bytes[selectableCardGuids.Count];
            for (int i = 0; i < selectableCardGuids.Count; i++)
                selectableCardUuidStrs[i] = selectableCardGuids[i].ToString();
        }
        else
            selectableCardUuidStrs = new FixedString128Bytes[0];

        BaseRpcTarget target = RpcTarget.Single(playerId, RpcTargetUse.Temp);
        SetSelectableCardsClientRpc(selectableCardUuidStrs, target);
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void SetSelectableCardsClientRpc(FixedString128Bytes[] selectableCardUuidStrs, RpcParams rpcParams) {
        List<Guid> selectableCardUuids = new List<Guid>();
        for (int i = 0; i < selectableCardUuidStrs.Length; i++)
            selectableCardUuids.Add(Guid.Parse(selectableCardUuidStrs[i].ToString()));
        OnSetSelectableFieldCards?.Invoke(this, selectableCardUuids);
    }

    public List<Guid> GetSelectableCardGuids(ulong playerId) {
        if (!IsServer)
            throw new Exception("Only the server can call the method GetSelectableCardGuids");

        List<Guid> selectableCardGuids = new List<Guid>();
        MatchPlayer player = duelManager.GetPlayerById(playerId);
        for (int i = 0; i < player.Creatures.Count; i++) {
            if (CanSelectAttacker(player, player.Creatures[i]) || CanSelectDefender(player, player.Creatures[i]))
                selectableCardGuids.Add(player.Creatures[i].Uuid);
        }
        OnGetSelectableFieldCards?.Invoke(this, new SelectableCardsEventArgs(playerId, selectableCardGuids));

        return selectableCardGuids;
    }

    private bool CanSelectAttacker(MatchPlayer player, CreatureCard card) {
        if (player.PlayerId != duelManager.GetCurrentPlayerTurn().PlayerId)
            return false;
        if (!combatStateManager.CurrentState.CanDeclareAttackers())
            return false;
        if (!player.ContainsCreatureUuid(card.Uuid))
            return false;
        if (!card.CanAttack())
            return false;
        PlayerCardCancelableEventArgs<CreatureCard> args = new PlayerCardCancelableEventArgs<CreatureCard>(player.PlayerId, card);
        EventBus.Instance.InvokeOnCanCreatureAttack(args);
        if (args.IsCanceled)
            return false;

        return true;
    }

    private bool CanSelectDefender(MatchPlayer player, CreatureCard card) {
        if (player.PlayerId == duelManager.GetCurrentPlayerTurn().PlayerId)
            return false;
        if (!combatStateManager.CurrentState.CanDeclareDefenders())
            return false;
        if (!player.ContainsCreatureUuid(card.Uuid))
            return false;
        if (!card.CanDefend())
            return false;
        PlayerCardCancelableEventArgs<CreatureCard> args = new PlayerCardCancelableEventArgs<CreatureCard>(player.PlayerId, card);
        EventBus.Instance.InvokeOnCanCreatureDefend(args);
        if (args.IsCanceled)
            return false;

        return true;
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