using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerPlayingFieldUIController : PlayingFieldUIController {
    [SerializeField] private PlayerPlayingFieldUI playingFieldUI;

    protected override void Start() {
        base.Start();
        stateManager.CombatPhase.OnCombatPhase += EnableSelectableCards;
        EventBus.Instance.OnReleaseCreatureFieldCardOverCombatArea += DeclareAttacker;
        EventBus.Instance.OnSelectAttackerToDefend += DeclareDefender;
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

    private void EnableSelectableCards(object sender, ulong playerId) {
        if (this.playerId != playerId)
            return;

        EnableSelectableCardsServerRpc();
    }

    [Rpc(SendTo.Server)]
    private void EnableSelectableCardsServerRpc(ServerRpcParams rpcParams = default) {
        List<Guid> selectableCardGuids = GetSelectableCardGuids(rpcParams.Receive.SenderClientId);
        BaseRpcTarget target = RpcTarget.Single(rpcParams.Receive.SenderClientId, RpcTargetUse.Temp);
        EnableSelectableCardsClientRpc(selectableCardGuids.ToArray(), target);
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void EnableSelectableCardsClientRpc(Guid[] selectableCardGuids, RpcParams rpcParams) {
        playingFieldUI.SetCardSelectableAll(false);
        for (int i = 0; i < selectableCardGuids.Length; i++)
            playingFieldUI.SetCardSelectable(selectableCardGuids[i]);
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
    private void DeclareAttackerServerRpc(ulong targetId, FixedString128Bytes cardUuidStr, ServerRpcParams rpcParams = default) {
        MatchPlayer initiator = duelManager.GetPlayerById(rpcParams.Receive.SenderClientId);
        Guid cardUuid = Guid.Parse(cardUuidStr.ToString());
        if (!combatStateManager.CurrentState.CanDeclareAttackers())
            return;
        if (initiator.ContainsCreatureUuid(cardUuid))
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
            playingFieldUI.AddCreatureFieldCard(creatures[i]);
    }

    public override bool ContainsCreature(Guid uuid) {
        return playingFieldUI.ContainsCreature(uuid);
    }
}
