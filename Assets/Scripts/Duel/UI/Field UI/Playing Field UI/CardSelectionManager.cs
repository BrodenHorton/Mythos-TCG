using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class CardSelectionManager : NetworkBehaviour {
    public event EventHandler<SelectableCardsEventArgs> OnGetSelectableCards;
    public event EventHandler<List<Guid>> OnSetSelectableCards;
    public event EventHandler OnClearSelectableCards;
    public event EventHandler<CardUIEventArgs<CardUI>> OnInspectCard;
    public event EventHandler OnCardDrag;
    public event EventHandler OnReleaseCardDrag;

    public static CardSelectionManager Instance { get; private set; }

    [SerializeField] private float dragOffset;
    private Camera cam;
    private bool isDragging;
    private CardUI draggingCard;
    private DuelManager duelManager;
    private ActionManager actionManager;
    private DuelStateManager stateManager;
    private CombatStateManager combatStateManager;
    private SpellChainManager spellChainManager;

    private void Awake() {
        if (Instance != null) {
            Debug.LogWarning("CardSelectionManager already exists in scene. Destroying redundant object.");
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
        stateManager = ServiceLocator.Get<DuelStateManager>();
        combatStateManager = ServiceLocator.Get<CombatStateManager>();
        spellChainManager = ServiceLocator.Get<SpellChainManager>();

        // Hand Card Selection Event Listeners
        stateManager.FirstMainPhase.OnFirstMainPhaseEnteredFinished += (sender, args) => SetSelectableCards(args);
        stateManager.CombatPhase.OnCombatPhaseEnteredFinished += (sender, args) => SetSelectableCards(args);
        stateManager.SecondMainPhase.OnSecondMainPhaseEnteredFinished += (sender, args) => SetSelectableCards(args);
        stateManager.EndPhase.OnEndPhasEnteredFinished += (sender, args) => ClearSelectableCards(args);
        spellChainManager.OnSpellChainEnd += SetSelectableCardsForActionFocusPlayers;
        EventBus.Instance.OnManaCountChangedFinished += (sender, args) => SetSelectableCards(args.PlayerId);
        actionManager.OnActionStateChanged +=  SetSelectableCardsForActionFocusPlayers;

        // Field Card Selection Event Listeners
        combatStateManager.DeclareAttackersState.OnDeclareAttackersStateEnteredFinished += (sender, args) => {
            SetSelectableCardsForActionFocusPlayers();
        };
        combatStateManager.DeclareDefendersState.OnDeclareDefendersEnteredFinished += (sender, args) => {
            SetSelectableCardsForActionFocusPlayers();
        };
        actionManager.OnActionStateChanged += SetSelectableCardsForActionFocusPlayers;
        EventBus.Instance.OnPostDeclareAttacker += SetSelectableCardsForActionFocusPlayers;
        EventBus.Instance.OnPostDeclareDefender += SetSelectableCardsForActionFocusPlayers;
        EventBus.Instance.OnPostUndeclareAttacker += SetSelectableCardsForActionFocusPlayers;
        EventBus.Instance.OnPostUndeclareDefender += SetSelectableCardsForActionFocusPlayers;

        PlayerInputActions playerInputActions = GameInputManager.Instance.PlayerInputActions;
        playerInputActions.Player.Select.started += SelectCard;
        playerInputActions.Player.Select.canceled += ReleaseCardDrag;
        playerInputActions.Player.Inspect.started += InspectFieldCard;
    }

    public override void OnNetworkDespawn() {
        base.OnNetworkDespawn();

        PlayerInputActions playerInputActions = GameInputManager.Instance.PlayerInputActions;
        playerInputActions.Player.Select.started -= SelectCard;
        playerInputActions.Player.Select.canceled -= ReleaseCardDrag;
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
        if (!IsServer)
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
        OnSetSelectableCards?.Invoke(this, selectableCardUuids);
    }

    public List<Guid> GetSelectableCardGuids(ulong playerId) {
        if (!IsServer)
            throw new Exception("Only the server can call the method GetSelectableCardGuids");

        List<Guid> selectableCardGuids = new List<Guid>();
        MatchPlayer player = duelManager.GetPlayerById(playerId);
        // Get Selectable Hand Cards
        for (int i = 0; i < player.Hand.Count; i++) {
            if (player.Hand[i].IsPlayable(duelManager, stateManager, spellChainManager, player))
                selectableCardGuids.Add(player.Hand[i].Uuid);
        }
        // Get Selectable Field Cards
        for (int i = 0; i < player.Creatures.Count; i++) {
            if (CanSelectAttacker(player, player.Creatures[i]) || CanSelectDefender(player, player.Creatures[i]))
                selectableCardGuids.Add(player.Creatures[i].Uuid);
        }
        OnGetSelectableCards?.Invoke(this, new SelectableCardsEventArgs(playerId, selectableCardGuids));

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

    private void ClearSelectableCards(ulong playerId) {
        if (!IsServer)
            throw new Exception("Only the server can call the method ClearSelectableCards");

        BaseRpcTarget target = RpcTarget.Single(playerId, RpcTargetUse.Temp);
        SetSelectableCardsClientRpc(new FixedString128Bytes[0], target);
    }

    private void SelectCard(InputAction.CallbackContext context) {
        if (!context.started)
            return;
        if (!CardUIRaycast(out CardUI cardUI))
            return;
        if (!cardUI.IsSelectable)
            return;

        cardUI.SelectCard(out bool canDragCard);

        if(canDragCard) {
            OnCardDrag?.Invoke(this, EventArgs.Empty);
            cardUI.StartCardDrag();
            isDragging = true;
            draggingCard = cardUI;
            draggingCard.transform.position = new Vector3(draggingCard.transform.position.x,
                                                          draggingCard.transform.position.y + dragOffset,
                                                          draggingCard.transform.position.z);
        }
    }

    private void ReleaseCardDrag(InputAction.CallbackContext context) {
        if (!context.canceled)
            return;
        if (!isDragging)
            return;

        CardUI cardUI = draggingCard;
        ResetCardDragging();
        OnReleaseCardDrag?.Invoke(this, EventArgs.Empty);
        cardUI.ReleaseCardDrag();
    }

    private void InspectFieldCard(InputAction.CallbackContext context) {
        if (!context.started)
            return;
        if (!CardUIRaycast(out CardUI cardUI))
            return;

        // TODO: Implement inspecting cards. Could implemnt a CardUI interface that is implemented
        // by hand and field cards
    }

    private bool CardUIRaycast(out CardUI cardUI) {
        cardUI = null;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);
        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        foreach (RaycastHit hit in hits) {
            CardCollisionPointer fieldCardCollisionPointer;
            if (hit.collider.TryGetComponent(out fieldCardCollisionPointer)) {
                cardUI = fieldCardCollisionPointer.GetCardUI();
                return true;
            }
        }

        return false;
    }

    public void ResetCardDragging() {
        isDragging = false;
        draggingCard = null;
    }

    public bool IsDragging { get { return isDragging; } }
}