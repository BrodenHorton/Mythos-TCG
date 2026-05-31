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
        EventBus.Instance.OnSelectAttackerToDefendUI += DeclareDefender;
        actionManager.OnActionStateChanged += (sender, args) => {
            SetSelectableCardsServerRpc();
        };
    }

    public override void Init(ulong playerId) {
        this.playerId = playerId;
        playingFieldUI.Init(playerId);
    }

    public override void PlayCreatureCard(CreatureCardPayload card) {
        playingFieldUI.PlayCreatureCard(card);
        SetSelectableCardsServerRpc();
    }

    public override void PlayDomainCard(DomainCardPayload card) {
        playingFieldUI.PlayDomainCard(card);
        SetSelectableCardsServerRpc();
    }

    public override void RemoveCreature(Guid cardUuid) {
        playingFieldUI.RemoveCreature(cardUuid);
    }

    public override CreatureFieldCardUI ReleaseCreature(Guid cardUuid) {
        return playingFieldUI.ReleaseCreature(cardUuid);
    }

    public override void UpdateCreatureFieldCard(CreatureCardPayload card) {
        playingFieldUI.UpdateCreatureFieldCard(card);
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
        if(combatManager.IsCreatureInCombat(card.Uuid))
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
        if (combatManager.IsCreatureInCombat(card.Uuid))
            return false;
        if (!card.CanDefend())
            return false;
        PlayerCardCancelableEventArgs<CreatureCard> args = new PlayerCardCancelableEventArgs<CreatureCard>(player.PlayerId, card);
        EventBus.Instance.InvokeOnCanCreatureDefend(args);
        if (args.IsCanceled)
            return false;

        return true;
    }

    private void DeclareAttacker(object sender, CombatFieldCardEventArgs<CreatureFieldCardUI> args) {
        DeclareAttackerServerRpc(args.CombatFieldUI.TargetPlayerId, args.CardUI.CardUuid.ToString());
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

        EventBus.Instance.InvokeOnDeclareAttacker(new CombatCreatureEventArgs(initiator.PlayerId, targetId, creatureCard));
        InvokeOnDeclareAttackerPayloadClientRpc(initiator.PlayerId, targetId, new CreatureCardPayload(creatureCard));
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void InvokeOnDeclareAttackerPayloadClientRpc(ulong initiatorId, ulong targetId, CreatureCardPayload cardPayload) {
        EventBus.Instance.InvokeOnDeclareAttackerFinished(new CombatCreaturePayloadEventArgs(initiatorId, targetId, cardPayload));
    }

    private void DeclareDefender(object sender, SelectAttackerToDefendUIEventArgs args) {
        TcgLogger.Log("DeclareDefender Entered");
        DeclareDefenderServerRpc(args.CombatFieldUI.TargetPlayerId, args.Attacker.CardUuid.ToString(), args.Defender.CardUuid.ToString());
    }

    [Rpc(SendTo.Server)]
    private void DeclareDefenderServerRpc(ulong targetId, FixedString128Bytes attackerUuidStr, FixedString128Bytes defenderUuidStr) {
        MatchPlayer initiator = duelManager.GetCurrentPlayerTurn();
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

        CanDefendEventArgs args = new CanDefendEventArgs(initiator.PlayerId, targetId, attacker, defender);
        EventBus.Instance.InvokeOnSelectAttackerToDefend(args);
        if (!args.CanDefend)
            return;

        EventBus.Instance.InvokeOnDeclareDefender(new CreatureCombatEventArgs(initiator.PlayerId, targetId, attacker, defender));
        InvokeOnDeclareDefenderPayloadClientRpc(initiator.PlayerId, targetId, new CreatureCardPayload(attacker), new CreatureCardPayload(defender));
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void InvokeOnDeclareDefenderPayloadClientRpc(ulong initiatorId, ulong targetId, CreatureCardPayload attacker, CreatureCardPayload defender) {
        EventBus.Instance.InvokeOnDeclareDefenderFinished(new CreatureCombatPayloadEventArgs(initiatorId, targetId, attacker, defender));
    }

    public override void GetCreatureCardsFromCombat(List<CreatureFieldCardUI> creatures) {
        for (int i = 0; i < creatures.Count; i++)
            playingFieldUI.AddCreatureFieldCard(creatures[i]);
    }

    public override bool ContainsCreature(Guid uuid) {
        return playingFieldUI.ContainsCreature(uuid);
    }
}
