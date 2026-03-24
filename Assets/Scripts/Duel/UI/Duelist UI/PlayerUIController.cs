using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUIController : DuelistUIController {
    [SerializeField] private PlayerUI playerUI;
    private Camera cam;

    private DuelManager duelManager;
    private PlayerInputActions playerInputActions;
    private HandCardUI previousSelection;

    private void Awake() {
        duelManager = FindFirstObjectByType<DuelManager>();
        if (duelManager == null)
            throw new Exception("Could not find DuelManager object");

        playerInputActions = new PlayerInputActions();
        playerInputActions.Enable();
        playerInputActions.Player.Select.performed += SelectCard;
        previousSelection = null;
    }

    private void Start() {
        DuelStateManager stateManager = FindFirstObjectByType<DuelStateManager>();
        if (stateManager == null)
            throw new Exception("Could not find DuelStateManager object");

        cam = Camera.main;

        //EventBus.OnCreatureCardSelectedForPlay += PlayCreatureCard;
        stateManager.FirstMainPhase.OnFirstMainPhase += SetSelectableCards;
        stateManager.CombatPhase.OnCombatPhase += HideSelectionBorders;
        stateManager.SecondMainPhase.OnSecondMainPhase += SetSelectableCards;
        stateManager.EndPhase.OnEndPhase += HideAllClientsSelectionBorders;
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

    public override void SetLifePoints(int lifePoints) {
        playerUI.SetLifePoints(lifePoints);
    }

    public override void SetManaCount(int manaCount) {
        playerUI.SetManaCount(manaCount);
        SetSelectableCardsAfterManaCountChanged();
    }

    public override void DrawCard(Card card) {
        playerUI.DrawCard(card);
    }

    public override void RemoveCardFromHand(int handIndex) {
        playerUI.RemoveCardFromHand(handIndex);
    }

    private void SetSelectableCards(object sender, PlayerEventArgs args) {
        if (player.PlayerId != args.Player.PlayerId)
            return;

        playerUI.SetSelectableCards(player);
    }

    private void SetSelectableCardsAfterManaCountChanged() {
        if (player.PlayerId != NetworkManager.Singleton.LocalClientId)
            return;
        // TODO: Add a boolean to tell if we are in a state that we can play selectable cards in

        playerUI.SetSelectableCards(player);
    }

    private void HideSelectionBorders(object sender, PlayerEventArgs args) {
        if (player.PlayerId != NetworkManager.Singleton.LocalClientId)
            return;
        if (player.PlayerId != args.Player.PlayerId)
            return;

        HideSelectionBorders();
    }

    private void HideSelectionBorders() {
        playerUI.SetBorderVisibilityAll(false);
    }

    private void HideAllClientsSelectionBorders(object sender, PlayerEventArgs args) {
        HideAllClientsSelectionBordersServerRpc();
    }

    [Rpc(SendTo.Server)]
    private void HideAllClientsSelectionBordersServerRpc() {
        HideAllClientsSelectionBordersClientRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void HideAllClientsSelectionBordersClientRpc() {
        playerUI.SetBorderVisibilityAll(false);
    }

    private void SelectCard(InputAction.CallbackContext context) {
        if (!context.performed)
            return;
        if (player.PlayerId != duelManager.GetCurrentPlayerTurn().PlayerId)
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

        PlayCardFromHand(player, cardIndex);
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
        playerUI.SetDefaultCardPositions();
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

    public void PlayCardFromHand(MatchPlayer player, int handIndex) {
        if (player.Hand.Count <= handIndex)
            return;

        player.Hand[handIndex].PlayCardFromHand(player, handIndex);
    }

    [Rpc(SendTo.Server)]
    private void PlayCardFromHandServerRpc(int playerIndex, CreatureCardNetworkSerializable cardNetworkSerializableObject, int handIndex) {
        PlayCreatureCardFromHandClientRpc(playerIndex, cardNetworkSerializableObject, handIndex);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void PlayCreatureCardFromHandClientRpc(int playerIndex, CreatureCardNetworkSerializable cardNetworkSerializableObject, int handIndex) {
        //MatchPlayer player = Players[playerIndex];
        //CreatureCard card = new CreatureCard(cardNetworkSerializableObject);
        //player.PlayCreatureCardFromHand(card, handIndex);
    }
}
