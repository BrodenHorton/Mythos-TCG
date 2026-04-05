using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerUIController : DuelistUIController {
    [SerializeField] private PlayerUI playerUI;
    
    private DuelManager duelManager;

    private void Start() {
        duelManager = FindFirstObjectByType<DuelManager>();
        if (duelManager == null)
            throw new Exception("Could not find DuelManager object");
        DuelStateManager stateManager = FindFirstObjectByType<DuelStateManager>();
        if (stateManager == null)
            throw new Exception("Could not find DuelStateManager object");

        playerUI.OnSelectingCardDrag += SelectCardDrag;
        EventBus.OnHandCardEnteringPlayingField += PlayHandCard;
        stateManager.FirstMainPhase.OnFirstMainPhase += SetSelectableCards;
        stateManager.CombatPhase.OnCombatPhase += HideSelectionBorders;
        stateManager.SecondMainPhase.OnSecondMainPhase += SetSelectableCards;
        stateManager.EndPhase.OnEndPhase += HideAllClientsSelectionBorders;
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

    private void SelectCardDrag(object sender, HandCardDragEventArgs args) {
        if (player.PlayerId != duelManager.GetCurrentPlayerTurn().PlayerId) {
            args.IsCancelled = true;
            return;
        }
        if (args.CardIndex < 0 || args.CardIndex >= player.Hand.Count) {
            args.IsCancelled = true;
            throw new Exception("Dragging card index out of bounds for player hand: " + args.CardIndex);
        }
        if (!player.Hand[args.CardIndex].IsPlayable(duelManager, player)) {
            args.IsCancelled = true;
            return;
        }
    }

    private void PlayHandCard(object sender, HandCardEnteringPlayingFieldEventArgs args) {
        if (player.PlayerId != duelManager.GetCurrentPlayerTurn().PlayerId)
            return;
        if (args.CardIndex < 0 || args.CardIndex >= player.Hand.Count)
            throw new Exception("Dragging card index out of bounds for player hand: " + args.CardIndex);
        if (!player.Hand[args.CardIndex].IsPlayable(duelManager, player))
            return;

        duelManager.PlayCardFromHand(player, args.CardIndex);
    }

    public override DuelistUI GetDuelistUI() {
        return playerUI;
    }
}
