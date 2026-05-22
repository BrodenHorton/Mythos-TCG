using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerUIController : DuelistUIController {
    [SerializeField] private PlayerUI playerUI;
    
    private DuelManager duelManager;
    private DuelStateManager stateManager;
    private ActionManager actionManager;
    private SpellChainManager spellChainManager;

    private void Start() {
        duelManager = ServiceLocator.Get<DuelManager>();
        stateManager = ServiceLocator.Get<DuelStateManager>();
        actionManager = ServiceLocator.Get<ActionManager>();
        spellChainManager = ServiceLocator.Get<SpellChainManager>();

        EventBus.Instance.OnPlayHandCard += PlayHandCard;
        stateManager.FirstMainPhase.OnFirstMainPhase += SetSelectableCards;
        stateManager.CombatPhase.OnCombatPhase += SetSelectableCards;
        stateManager.SecondMainPhase.OnSecondMainPhase += SetSelectableCards;
        stateManager.EndPhase.OnEndPhase += DisableSelectableCards;
        EventBus.Instance.OnManaCountChanged += (sender, args) => {
            if (playerId == args.PlayerId)
                SetSelectableCardsServerRpc();
        };
        actionManager.OnActionStateChanged += (sender, args) => {
            SetSelectableCardsServerRpc();
        };
        spellChainManager.OnSpellChainFinished += (sender, args) => {
            SetSelectableCardsServerRpc();
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

    public override void DrawCard(CardPayload card) {
        playerUI.DrawCard(card);
    }

    public override void RemoveCardFromHand(Guid cardUuid) {
        playerUI.RemoveCardFromHand(cardUuid);
    }

    private void SetSelectableCards(object sender, ulong playerId) {
        if (this.playerId != playerId)
            return;

        SetSelectableCardsServerRpc();
    }

    [Rpc(SendTo.Server)]
    private void SetSelectableCardsServerRpc(RpcParams rpcParams = default) {
        ulong clientPlayerId = rpcParams.Receive.SenderClientId;
        FixedString128Bytes[] selectableCardUuidStrs;
        if (actionManager.ActionFocusPlayerIds.Contains(clientPlayerId)) {
            List<Guid> selectableCardGuids = GetSelectableCardGuids(clientPlayerId);
            selectableCardUuidStrs = new FixedString128Bytes[selectableCardGuids.Count];
            for (int i = 0; i < selectableCardGuids.Count; i++)
                selectableCardUuidStrs[i] = selectableCardGuids[i].ToString();
        }
        else
            selectableCardUuidStrs = new FixedString128Bytes[0];

        BaseRpcTarget target = RpcTarget.Single(clientPlayerId, RpcTargetUse.Temp);
        SetSelectableCardsClientRpc(selectableCardUuidStrs, target);
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void SetSelectableCardsClientRpc(FixedString128Bytes[] selectableCardUuidStrs, RpcParams rpcParams) {
        playerUI.SetCardSelectableAll(false);
        for (int i = 0; i < selectableCardUuidStrs.Length; i++)
            playerUI.SetCardSelectable(Guid.Parse(selectableCardUuidStrs[i].ToString()));
    }

    private void DisableSelectableCards(object sender, ulong playerId) {
        if (this.playerId != playerId)
            return;

        playerUI.SetCardSelectableAll(false);
    }

    public List<Guid> GetSelectableCardGuids(ulong clientPlayerId) {
        if (!IsServer)
            throw new Exception("Attempting the call GetSelectableCardGuids from a client");

        List<Guid> selectableCardGuids = new List<Guid>();
        MatchPlayer player = duelManager.GetPlayerById(clientPlayerId);
        for (int i = 0; i < player.Hand.Count; i++) {
            if (player.Hand[i].IsPlayable(duelManager, stateManager, spellChainManager, player))
                selectableCardGuids.Add(player.Hand[i].Uuid);
        }

        return selectableCardGuids;
    }

    private void PlayHandCard(object sender, PlayerCardUuidEventArgs args) {
        PlayHandCardServerRpc(args.PlayerId, args.CardUuid.ToString());
    }

    [Rpc(SendTo.Server)]
    private void PlayHandCardServerRpc(ulong playerId, FixedString128Bytes handCardUuidStr) {
        MatchPlayer player = duelManager.GetPlayerById(playerId);
        Guid handCardUuid = Guid.Parse(handCardUuidStr.ToString());
        if (!actionManager.ActionFocusPlayerIds.Contains(playerId))
            return;
        if (!player.ContainsHandCardeUuid(handCardUuid))
            return;
        if (!player.GetHandCardByUuid(handCardUuid).IsPlayable(duelManager, stateManager, spellChainManager, player))
            return;

        duelManager.PlayCardFromHand(playerId, handCardUuid);
    }

    public override DuelistUI GetDuelistUI() {
        return playerUI;
    }
}
