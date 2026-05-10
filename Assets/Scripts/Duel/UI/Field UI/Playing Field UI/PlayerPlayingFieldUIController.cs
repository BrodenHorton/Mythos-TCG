using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerPlayingFieldUIController : PlayingFieldUIController {
    [SerializeField] private PlayerPlayingFieldUI playingFieldUI;

    protected override void Start() {
        base.Start();
        playingFieldUI.OnSelectingCardDrag += SelectCardDrag;
        EventBus.Instance.OnReleaseCreatureFieldCardOverCombatArea += DeclareAttacker;
        EventBus.Instance.OnSelectAttackerToDefend += DeclareDefender;
    }

    public override void Init(ulong playerId) {
        this.playerId = playerId;
        playingFieldUI.Init(playerId);
    }

    public override void PlayCreatureCard(CreatureCard card) {
        playingFieldUI.PlayCreatureCard(playerId, card);
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

    private void SelectCardDrag(object sender, FieldCardDragEventArgs<CreatureFieldCardUI> args) {
        if (!CanSelectAttacker(args) && !CanSelectDefender(args))
            args.IsCancelled = true;
    }

    private bool CanSelectAttacker(FieldCardDragEventArgs<CreatureFieldCardUI> args) {
        if (playerId != duelManager.LocalClientPlayer)
            return false;
        if (!duelManager.IsLocalClientPlayerTurn())
            return false;
        if (!combatStateManager.CurrentState.CanDeclareAttackers())
            return false;
        if (args.CardUI == null)
            return false;
        if (!playerId.ContainsCreatureUuid(args.CardUI.CardUuid))
            return false;
        CreatureCard creatureCard = playerId.GetCreatureByUuid(args.CardUI.CardUuid);
        if (creatureCard == null)
            return false;
        if (!creatureCard.CanAttack())
            return false;

        return true;
    }

    private bool CanSelectDefender(FieldCardDragEventArgs<CreatureFieldCardUI> args) {
        if (playerId != duelManager.LocalClientPlayer)
            return false;
        if (duelManager.IsLocalClientPlayerTurn())
            return false;
        if (!combatStateManager.CurrentState.CanDeclareDefenders())
            return false;
        if (args.CardUI == null)
            return false;
        if (!playerId.ContainsCreatureUuid(args.CardUI.CardUuid))
            return false;
        CreatureCard creatureCard = playerId.GetCreatureByUuid(args.CardUI.CardUuid);
        if (creatureCard == null)
            return false;

        return true;
    }

    private void DeclareAttacker(object sender, CreatureFieldCardEnteringCombatFieldEventArgs args) {
        if (playerId != duelManager.LocalClientPlayer)
            return;
        if (!combatStateManager.CurrentState.CanDeclareAttackers())
            return;
        CreatureCard creatureCard = playerId.GetCreatureByUuid(args.CardUI.CardUuid);
        if (creatureCard == null)
            return;
        if (!creatureCard.CanAttack())
            return;

        MatchPlayer initiator = duelManager.GetCurrentPlayerTurn();
        MatchPlayer target = duelManager.GetPlayerById(args.TargetPlayerId);
        DeclareAttackerServerRpc(duelManager.GetPlayerIndex(initiator), duelManager.GetPlayerIndex(target), creatureCard.Uuid.ToString());
    }

    [Rpc(SendTo.Server)]
    private void DeclareAttackerServerRpc(int initiatorIndex, int targetIndex, FixedString128Bytes cardUuidStr) {
        DeclareAttackerClientRpc(initiatorIndex, targetIndex, cardUuidStr);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void DeclareAttackerClientRpc(int initiatorIndex, int targetIndex, FixedString128Bytes cardUuidStr) {
        Guid cardUuid = Guid.Parse(cardUuidStr.ToString());
        CreatureCard creatureCard = duelManager.Players[initiatorIndex].GetCreatureByUuid(cardUuid);
        EventBus.Instance.InvokeOnDeclareAttacker(new DeclareAttackerEventArgs(duelManager.Players[initiatorIndex], duelManager.Players[targetIndex], creatureCard));
    }

    private void DeclareDefender(object sender, SelectAttackerToDefendEventArgs args) {
        if (playerId != duelManager.LocalClientPlayer)
            return;
        if (!combatStateManager.CurrentState.CanDeclareDefenders())
            return;
        if (args.CombatFieldUI.TargetPlayerId != playerId)
            return;
        if (!playerId.ContainsCreatureUuid(args.Defender.CardUuid))
            return;
        CreatureCard defender = playerId.GetCreatureByUuid(args.Defender.CardUuid);
        if (defender == null)
            return;
        if (!defender.CanDefend())
            return;

        MatchPlayer initiator = duelManager.GetCurrentPlayerTurn();
        MatchPlayer target = duelManager.GetPlayerById(args.TargetPlayerId);
        DeclareDefenderServerRpc(duelManager.GetPlayerIndex(initiator), duelManager.GetPlayerIndex(target), args.Attacker.CardUuid.ToString(), defender.Uuid.ToString());
    }

    [Rpc(SendTo.Server)]
    private void DeclareDefenderServerRpc(int initiatorIndex, int targetIndex, FixedString128Bytes attackerCardUuidStr, FixedString128Bytes defenderCardUuidStr) {
        DeclareDefenderClientRpc(initiatorIndex, targetIndex, attackerCardUuidStr, defenderCardUuidStr);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void DeclareDefenderClientRpc(int initiatorIndex, int targetIndex, FixedString128Bytes attackerCardUuidStr, FixedString128Bytes defenderCardUuidStr) {
        Guid attackerUuid = Guid.Parse(attackerCardUuidStr.ToString());
        Guid defenderUuid = Guid.Parse(defenderCardUuidStr.ToString());
        CreatureCard attacker = duelManager.Players[initiatorIndex].GetCreatureByUuid(attackerUuid);
        CreatureCard defender = duelManager.Players[targetIndex].GetCreatureByUuid(defenderUuid);
        EventBus.Instance.InvokeOnDeclareDefender(new DeclareDefenderEventArgs(duelManager.Players[initiatorIndex], duelManager.Players[targetIndex], attacker, defender));
    }

    public override void GetCreatureCardsFromCombat(List<CreatureFieldCardUI> creatures) {
        for (int i = 0; i < creatures.Count; i++)
            playingFieldUI.AddCreatureFieldCard(playerId, creatures[i]);
    }

    public override bool ContainsCreature(Guid uuid) {
        return playingFieldUI.ContainsCreature(uuid);
    }
}
