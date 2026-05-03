using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerUIController : DuelistUIController {
    [SerializeField] private PlayerUI playerUI;
    
    private DuelManager duelManager;
    private DuelStateManager stateManager;
    private ActionManager actionManager;
    private SpellChainManager spellChainManager;
    private bool canSelectCards;

    private void Awake() {
        canSelectCards = false;
    }

    private void Start() {
        duelManager = FindFirstObjectByType<DuelManager>();
        if (duelManager == null)
            throw new Exception("Could not find DuelManager object");
        stateManager = FindFirstObjectByType<DuelStateManager>();
        if (stateManager == null)
            throw new Exception("Could not find DuelStateManager object");
        actionManager = FindFirstObjectByType<ActionManager>();
        if (actionManager == null)
            throw new Exception("Could not find ActionManager object");
        spellChainManager = FindFirstObjectByType<SpellChainManager>();
        if (spellChainManager == null)
            throw new Exception("Could not find SpellChainManager object");

        playerUI.OnSelectingCardDrag += SelectCardDrag;
        EventBus.OnHandCardEnteringPlayingField += PlayHandCard;
        stateManager.FirstMainPhase.OnFirstMainPhase += EnableSelectableCardsOnPlayerEvent;
        stateManager.CombatPhase.OnCombatPhase += DisableSelectableCardsOnPlayerEvent;
        stateManager.SecondMainPhase.OnSecondMainPhase += EnableSelectableCardsOnPlayerEvent;
        stateManager.EndPhase.OnEndPhase += DisableAllClientSelectableCards;
        EventBus.OnManaCountChanged += EnableSelectableCardsAfterManaCountChanged;
        actionManager.OnCanPerformActionChanged += SetSelectableCardsOnActionFocusChanged;
        spellChainManager.OnSpellChainFinished += SetSelectableCardsOnSpellChainFinished;
    }

    public override void Init(ulong playerId, int lifePoints, int manaCount) {
        this.playerId = playerId;
        playerUI.Init(playerId, lifePoints, manaCount);
    }

    public override void SetLifePoints(int lifePoints) {
        playerUI.SetLifePoints(lifePoints);
    }

    public override void SetManaCount(int manaCount) {
        playerUI.SetManaCount(manaCount);
    }

    public override void DrawCard(Card card) {
        playerUI.DrawCard(card);
    }

    public override void RemoveCardFromHand(int handIndex) {
        playerUI.RemoveCardFromHand(handIndex);
    }

    private void EnableSelectableCardsAfterManaCountChanged(object sender, ManaChangedEventArgs args) {
        if (playerId != args.Player.PlayerId)
            return;

        SetCanSelectCards(true);
    }

    private void SetSelectableCardsOnPlayerSpellChainTurn(object sender, PlayerEventArgs args) {
        if (playerId == args.Player.PlayerId)
            SetCanSelectCards(true);
        else
            SetCanSelectCards(false);
    }

    private void SetSelectableCardsOnSpellChainFinished(object sender, EventArgs args) {
        if (playerId == duelManager.GetCurrentPlayerTurn().PlayerId)
            SetCanSelectCards(true);
        else
            SetCanSelectCards(false);
    }

    private void SetSelectableCardsOnActionFocusChanged(object sender, EventArgs args) {
        if (actionManager.CanPerformAction)
            SetCanSelectCards(true);
        else
            SetCanSelectCards(false);
    }

    private void EnableSelectableCardsOnPlayerEvent(object sender, PlayerEventArgs args) {
        if (playerId != args.Player.PlayerId)
            return;

        SetCanSelectCards(true);
    }

    private void DisableSelectableCardsOnPlayerEvent(object sender, PlayerEventArgs args) {
        if (playerId != args.Player.PlayerId)
            return;

        SetCanSelectCards(false);
    }

    private void DisableAllClientSelectableCards(object sender, PlayerEventArgs args) {
        DisableAllClientsSelectionCardsServerRpc();
    }

    [Rpc(SendTo.Server)]
    private void DisableAllClientsSelectionCardsServerRpc() {
        DisableAllClientsSelectionCardsClientRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void DisableAllClientsSelectionCardsClientRpc() {
        SetCanSelectCards(false);
    }

    public void SetCanSelectCards(bool canSelectCards) {
        this.canSelectCards = canSelectCards;

        if (this.canSelectCards)
            ShowSelectionBorders();
        else
            HideSelectionBorders();
    }

    private void ShowSelectionBorders() {
        playerUI.SetBorderVisibilityAll(false);
        for (int i = 0; i < playerId.Hand.Count; i++) {
            if (playerId.Hand[i].IsPlayable(duelManager, stateManager, spellChainManager, playerId))
                playerUI.SetCardSelectable(playerId.Hand[i].Uuid);
        }
    }

    private void HideSelectionBorders() {
        playerUI.SetBorderVisibilityAll(false);
    }

    private void SelectCardDrag(object sender, HandCardDragEventArgs args) {
        if(!canSelectCards) {
            args.IsCancelled = true;
            return;
        }
        if (!actionManager.CanPerformAction) {
            args.IsCancelled = true;
            return;
        }
        if (args.CardIndex < 0 || args.CardIndex >= playerId.Hand.Count) {
            args.IsCancelled = true;
            throw new Exception("Dragging card index out of bounds for player hand: " + args.CardIndex);
        }
        if (!playerId.Hand[args.CardIndex].IsPlayable(duelManager, stateManager, spellChainManager, playerId)) {
            args.IsCancelled = true;
            return;
        }
    }

    private void PlayHandCard(object sender, HandCardEnteringPlayingFieldEventArgs args) {
        if (!actionManager.CanPerformAction)
            return;
        if (args.CardIndex < 0 || args.CardIndex >= playerId.Hand.Count)
            throw new Exception("Dragging card index out of bounds for player hand: " + args.CardIndex);
        if (!playerId.Hand[args.CardIndex].IsPlayable(duelManager, stateManager, spellChainManager, playerId))
            return;

        duelManager.PlayCardFromHand(playerId, args.CardIndex);
    }

    public override DuelistUI GetDuelistUI() {
        return playerUI;
    }
}
