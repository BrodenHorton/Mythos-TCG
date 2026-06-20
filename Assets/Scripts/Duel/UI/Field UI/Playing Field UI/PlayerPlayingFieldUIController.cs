using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerPlayingFieldUIController : PlayingFieldUIController {

    protected override void Start() {
        base.Start();
        EventBus.Instance.OnReleaseCreatureFieldCardOverCombatArea += DeclareAttacker;
        EventBus.Instance.OnSelectAttackerToDefendUI += DeclareDefender;
    }

    public override void Init(ulong playerId) {
        this.playerId = playerId;
        playingFieldUI.Init(playerId);
    }

    public override void PlayCreatureCard(CreatureCardPayload card) {
        playingFieldUI.PlayCreatureCard(card);
    }

    public override void PlayDomainCard(DomainCardPayload card) {
        playingFieldUI.PlayDomainCard(card);
    }

    public override void AddCreatureCard(CreatureFieldCardUI card) {
        playingFieldUI.AddCreatureFieldCard(card);
    }

    public override void AddCreatureCards(List<CreatureFieldCardUI> creatures) {
        for (int i = 0; i < creatures.Count; i++)
            playingFieldUI.AddCreatureFieldCard(creatures[i]);
    }

    public override void RemoveCreature(Guid cardUuid) {
        playingFieldUI.RemoveCreature(cardUuid);
    }

    public override CreatureFieldCardUI ReleaseCreature(Guid cardUuid) {
        return playingFieldUI.ReleaseCreature(cardUuid);
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

    public override bool ContainsCreature(Guid uuid) {
        return playingFieldUI.ContainsCreature(uuid);
    }
}
