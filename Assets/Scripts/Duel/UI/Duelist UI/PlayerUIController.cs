using System;
using System.Collections.Generic;
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
        EventBus.Instance.OnHandCardEnteringPlayingField += PlayHandCard;
        stateManager.FirstMainPhase.OnFirstMainPhase += EnableSelectableCards;
        stateManager.CombatPhase.OnCombatPhase += EnableSelectableCards;
        stateManager.SecondMainPhase.OnSecondMainPhase += EnableSelectableCards;
        stateManager.EndPhase.OnEndPhase += DisableSelectableCards;
        EventBus.Instance.OnManaCountChanged += (sender, args) => {
            if (playerId == args.PlayerId)
                EnableSelectableCardsServerRpc();
        };
        actionManager.OnActionStateChanged += (sender, args) => {
            if (args.HasActionFocus)
                EnableSelectableCardsServerRpc();
        };
        spellChainManager.OnSpellChainFinished += (sender, args) => {
            if(playerId == duelManager.GetCurrentPlayerTurn().PlayerId)
                EnableSelectableCardsServerRpc();
        };
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

    private void EnableSelectableCards(object sender, ulong playerId) {
        if (this.playerId != playerId)
            return;

        EnableSelectableCardsServerRpc();
    }

    [Rpc(SendTo.Server)]
    private void EnableSelectableCardsServerRpc(ServerRpcParams rpcParams = default) {
        List<Guid> selectableCardGuids = GetSelectableCardGuids(rpcParams.Receive.SenderClientId);
        BaseRpcTarget target = RpcTarget.Group(new List<ulong>() { rpcParams.Receive.SenderClientId }, RpcTargetUse.Temp);
        EnableSelectableCardsClientRpc(selectableCardGuids.ToArray(), target);
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void EnableSelectableCardsClientRpc(Guid[] selectableCardGuids, RpcParams rpcParams) {
        playerUI.SetBorderVisibilityAll(false);
        for (int i = 0; i < selectableCardGuids.Length; i++)
            playerUI.SetCardSelectable(selectableCardGuids[i]);
    }

    private void DisableSelectableCards(object sender, ulong playerId) {
        if (this.playerId != playerId)
            return;

        playerUI.SetBorderVisibilityAll(false);
    }

    public List<Guid> GetSelectableCardGuids(ulong clientPlayerId) {
        if (!IsServer)
            throw new Exception("Attempting the call GetSelectableCardGuids from a client");

        List<Guid> selectableCardGuids = new List<Guid>();
        MatchPlayer player = duelManager.GetPlayerById(clientPlayerId);
        for (int i = 0; i < player.Hand.Count; i++) {
            if (player.Hand[i].IsPlayable(duelManager, stateManager, spellChainManager, player))
                playerUI.SetCardSelectable(player.Hand[i].Uuid);
        }

        return selectableCardGuids;
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
