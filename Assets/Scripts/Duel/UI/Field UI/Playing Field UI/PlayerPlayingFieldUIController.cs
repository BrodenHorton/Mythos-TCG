using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerPlayingFieldUIController : PlayingFieldUIController {
    [SerializeField] private PlayerPlayingFieldUI playingFieldUI;

    protected override void Start() {
        base.Start();
        combatStateManager.DeclareAttackersState.OnStartDeclareAttackers += SetSelectableCards;
        combatStateManager.DeclareDefendersState.OnStartDeclareDefenders += SetSelectableCards;
        EventBus.Instance.OnReleaseCreatureFieldCardOverCombatArea += DeclareAttacker;
        EventBus.Instance.OnSelectAttackerToDefend += DeclareDefender;
        actionManager.OnActionStateChanged += (sender, args) => {
            SetSelectableCardsServerRpc();
        };
    }

    public override void Init(ulong playerId) {
        this.playerId = playerId;
        playingFieldUI.Init(playerId);
    }

    public override void PlayCreatureCard(CreatureCard card) {
        playingFieldUI.PlayCreatureCard(card);
    }

    public override void PlayDomainCard(DomainCard card) {
        playingFieldUI.PlayDomainCard(card);
    }

    public override void RemoveCreature(CreatureCard card) {
        playingFieldUI.RemoveCreature(card.Uuid);
    }

    public override void UpdateCreatureFieldCard(CreatureCard card) {
        playingFieldUI.UpdateCreatureFieldCard(card);
    }

    public override void TapCreature(CreatureCard card) {
        playingFieldUI.TapCreature(card);
    }

    public override void UntapCreature(CreatureCard card) {
        playingFieldUI.UntapCreature(card);
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
        playingFieldUI.SetCardSelectableAll(false);
        for (int i = 0; i < selectableCardUuidStrs.Length; i++) {
            Guid selectableCardUuid = Guid.Parse(selectableCardUuidStrs[i].ToString());
            playingFieldUI.SetCardSelectable(selectableCardUuid);
        }
    }

    private void DisableSelectableCards(object sender, ulong playerId) {
        if (this.playerId != playerId)
            return;

        playingFieldUI.SetCardSelectableAll(false);
    }

    public List<Guid> GetSelectableCardGuids(ulong clientPlayerId) {
        if (!IsServer)
            throw new Exception("Attempting the call GetSelectableCardGuids from a client");

        List<Guid> selectableCardGuids = new List<Guid>();
        MatchPlayer player = duelManager.GetPlayerById(clientPlayerId);
        for (int i = 0; i < player.Creatures.Count; i++) {
            if (CanSelectAttacker(player, player.Creatures[i]) || CanSelectDefender(player, player.Creatures[i]))
                selectableCardGuids.Add(player.Creatures[i].Uuid);
        }

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

        return true;
    }

    private void DeclareAttacker(object sender, CreatureFieldCardEnteringCombatFieldEventArgs args) {
        DeclareAttackerServerRpc(args.TargetPlayerId, args.CardUI.CardUuid.ToString());
    }

    [Rpc(SendTo.Server)]
    private void DeclareAttackerServerRpc(ulong targetId, FixedString128Bytes cardUuidStr, RpcParams rpcParams = default) {
        MatchPlayer initiator = duelManager.GetPlayerById(rpcParams.Receive.SenderClientId);
        Guid cardUuid = Guid.Parse(cardUuidStr.ToString());
        if (!combatStateManager.CurrentState.CanDeclareAttackers())
            return;
        if (!initiator.ContainsCreatureUuid(cardUuid))
            return;
        CreatureCard creatureCard = initiator.GetCreatureByUuid(cardUuid);
        if (creatureCard == null)
            return;
        if (!creatureCard.CanAttack())
            return;

        InvokeOnDeclareAttackerClientRpc(initiator.PlayerId, targetId, creatureCard);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void InvokeOnDeclareAttackerClientRpc(ulong initiatorId, ulong targetId, CreatureCard card) {
        EventBus.Instance.InvokeOnDeclareAttacker(new DeclareAttackerEventArgs(initiatorId, targetId, card));
    }

    private void DeclareDefender(object sender, SelectAttackerToDefendEventArgs args) {
        DeclareDefenderServerRpc(duelManager.GetCurrentPlayerTurn().PlayerId, args.TargetPlayerId, args.Attacker.CardUuid.ToString(), args.Defender.CardUuid.ToString());
    }

    [Rpc(SendTo.Server)]
    private void DeclareDefenderServerRpc(ulong initiatorId, ulong targetId, FixedString128Bytes attackerUuidStr, FixedString128Bytes defenderUuidStr) {
        MatchPlayer initiator = duelManager.GetPlayerById(initiatorId);
        MatchPlayer target = duelManager.GetPlayerById(targetId);
        Guid attackerUuid = Guid.Parse(attackerUuidStr.ToString());
        Guid defenderUuid = Guid.Parse(defenderUuidStr.ToString());
        if (!combatStateManager.CurrentState.CanDeclareDefenders())
            return;
        if (!target.ContainsCreatureUuid(defenderUuid))
            return;
        CreatureCard defender = target.GetCreatureByUuid(defenderUuid);
        if (defender == null)
            return;
        if (!defender.CanDefend())
            return;
        if (!initiator.ContainsCreatureUuid(attackerUuid))
            return;
        CreatureCard attacker = initiator.GetCreatureByUuid(attackerUuid);
        if (attacker == null)
            return;

        InvokeOnDeclareDefenderClientRpc(initiatorId, targetId, attacker, defender);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void InvokeOnDeclareDefenderClientRpc(ulong initiatorId, ulong targetId, CreatureCard attacker, CreatureCard defender) {
        EventBus.Instance.InvokeOnDeclareDefender(new DeclareDefenderEventArgs(initiatorId, targetId, attacker, defender));
    }

    public override void GetCreatureCardsFromCombat(List<CreatureFieldCardUI> creatures) {
        for (int i = 0; i < creatures.Count; i++)
            playingFieldUI.AddCreatureFieldCard(creatures[i]);
    }

    public override bool ContainsCreature(Guid uuid) {
        return playingFieldUI.ContainsCreature(uuid);
    }
}
