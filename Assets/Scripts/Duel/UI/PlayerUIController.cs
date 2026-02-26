using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUIController : DuelistUIController {
    [SerializeField] private PlayerUI playerUI;

    private DuelManager duelManager;
    private Camera cam;
    private PlayerInputActions playerInputActions;
    private HandCardUI previousSelection;

    private void Awake() {
        duelManager = FindFirstObjectByType<DuelManager>();
        if (duelManager == null)
            throw new Exception("Could not find DuelManager object");

        cam = Camera.main;
        playerInputActions = new PlayerInputActions();
        playerInputActions.Enable();
        playerInputActions.Player.Select.performed += SelectCard;
        previousSelection = null;
    }

    private void Start() {
        DuelStateManager stateManager = FindFirstObjectByType<DuelStateManager>();
        if (stateManager == null)
            throw new Exception("Could not find DuelStateManager object");

        EventBus.OnLifePointsChanged += SetLifePoints;
        EventBus.OnManaCountChanged += SetManaCount;
        EventBus.OnCreatureCardDrawn += DrawCreatureCard;
        EventBus.OnSpellCardDrawn += DrawSpellCard;
        stateManager.DrawPhase.OnDrawPhase += HideBorderAll;
        stateManager.MainPhase.OnMainPhase += SetSelectableCards;
    }

    private void Update() {
        HandCardUI handCardUI = HoverDetection();
        if(handCardUI == null && previousSelection != null) {
            ExitHoverHand();
            previousSelection = null;
        }
        else if(handCardUI != null && playerUI.ContainsCard(handCardUI)) {
            if (previousSelection == null) {
                HoverHand(handCardUI);
                previousSelection = handCardUI;
            }
            else if(handCardUI != previousSelection) {
                ExitHoverCard(previousSelection);
                HoverCard(handCardUI);
                previousSelection = handCardUI;
            }
        }
    }

    public override void Init(MatchPlayer player) {
        this.player = player;
        playerUI.Init(player);
    }

    private void DrawCreatureCard(object sender, PlayerCreatureCardEventArgs args) {
        if (args.Player.Uuid == player.Uuid)
            playerUI.DrawCreatureCard(args.Card);
    }

    private void DrawSpellCard(object sender, PlayerSpellCardEventArgs args) {
        if (args.Player.Uuid == player.Uuid)
            playerUI.DrawSpellCard(args.Card);
    }

    private void SetLifePoints(object sender, LifePointsChangedEventArgs args) {
        if (args.Player.Uuid == player.Uuid)
            playerUI.SetLifePoints(args.LifePoints);
    }

    private void SetManaCount(object sender, ManaChangedEventArgs args) {
        if (args.Player.Uuid == player.Uuid)
            playerUI.SetManaCount(args.CurrentMana);
    }

    private void SetSelectableCards(object sender, PlayerEventArgs args) {
        if (player.Uuid != duelManager.GetCurrentPlayerTurn().Uuid)
            return;

        playerUI.SetSelectableCards(player);
    }

    private void HideBorderAll(object sender, EventArgs args) {
        playerUI.SetBorderVisibilityAll(false);
    }

    private void SelectCard(InputAction.CallbackContext context) {
        if (!context.performed)
            return;
        if (player.Uuid != duelManager.GetCurrentPlayerTurn().Uuid)
            return;
        HandCardUI handCardUI = RaycastColliderCheck();
        if (handCardUI == null)
            return;
        if (!playerUI.ContainsCard(handCardUI))
            return;
        int cardIndex = playerUI.IndexOf(handCardUI);
        if (cardIndex == -1)
            return;
        if (!player.Hand[cardIndex].IsPlayable(duelManager, player))
            return;

        duelManager.PlayCardFromHand(player, cardIndex);
        handCardUI.Select();
        playerUI.SetSelectableCards(player);
    }

    private HandCardUI RaycastColliderCheck() {
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

    private HandCardUI HoverDetection() {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);
        if(hits.Length > 0)
            Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        foreach (RaycastHit hit in hits) {
            if (hit.collider.GetComponent<HandCardCollisionPointer>())
                return hit.collider.GetComponent<HandCardCollisionPointer>().HandCardUI;
        }

        return null;
    }

    private void HoverHand(HandCardUI card) {
        playerUI.InspectHand();
        HoverCard(card);
    }

    private void ExitHoverHand() {
        playerUI.DefaultCardPositions();
    }

    private void HoverCard(HandCardUI card) {
        playerUI.HoverCard(card);
    }

    private void ExitHoverCard(HandCardUI card) {
        playerUI.ExitHoverCard(card);
    }

    public override DuelistUI GetDuelistUI() {
        return playerUI;
    }
}
