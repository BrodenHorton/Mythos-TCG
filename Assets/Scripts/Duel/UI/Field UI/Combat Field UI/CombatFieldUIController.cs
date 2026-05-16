using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class CombatFieldUIController : NetworkBehaviour {
    [SerializeField] private CombatFieldUI combatFieldUI;

    protected ulong targetPlayerId;
    protected DuelManager duelManager;
    protected DuelStateManager stateManager;
    protected CombatStateManager combatStateManager;

    protected virtual void Start() {
        duelManager = FindFirstObjectByType<DuelManager>();
        if (duelManager == null)
            throw new Exception("Could not find DuelManager object");
        stateManager = FindFirstObjectByType<DuelStateManager>();
        if (stateManager == null)
            throw new Exception("Could not find DuelStateManager object");
        combatStateManager = FindFirstObjectByType<CombatStateManager>();
        if (combatStateManager == null)
            throw new Exception("Could not find CombatStateManager object");

        combatFieldUI.OnSelectFieldCard += UndeclareAttacker;
        combatFieldUI.OnSelectFieldCard += UndeclareDefender;
    }

    public override void OnNetworkDespawn() {
        combatFieldUI.OnSelectFieldCard -= UndeclareAttacker;
        combatFieldUI.OnSelectFieldCard -= UndeclareDefender;
    }

    public void Init(ulong playerId) {
        targetPlayerId = playerId;
        combatFieldUI.Init(playerId);
    }

    public void AddAttacker(CreatureCard attacker) {
        combatFieldUI.AddAttacker(attacker);
    }

    public void AddDefender(CreatureCard defender, Guid attackerCardUuid) {
        combatFieldUI.AddDefender(defender, attackerCardUuid);
    }

    public void RemoveAttacker(CreatureCard attacker) {
        combatFieldUI.RemoveAttacker(attacker.Uuid);
    }

    public void RemoveDefender(CreatureCard defender) {
        combatFieldUI.RemoveDefender(defender.Uuid);
    }

    public void UpdateCreatureFieldCard(CreatureCard card) {
        if (!ContainsAttacker(card.Uuid) && !ContainsDefender(card.Uuid))
            return;

        combatFieldUI.UpdateCreatureFieldCard(card);
    }

    private void UndeclareAttacker(object sender, CombatFieldCardSelectEventArgs args) {
        if (args.CombatFieldUI != combatFieldUI)
            return;
        if (args.CardUI == null)
            return;

        UndeclareAttackerServerRpc(targetPlayerId, args.CardUI.CardUuid.ToString());
    }

    [Rpc(SendTo.Server)]
    private void UndeclareAttackerServerRpc(ulong targetId, FixedString128Bytes creatureCardUuidStr) {
        if (!combatStateManager.CurrentState.CanDeclareAttackers())
            return;
        MatchPlayer initiator = duelManager.GetCurrentPlayerTurn();
        Guid creatureCardUuid = Guid.Parse(creatureCardUuidStr.ToString());
        if (!initiator.ContainsCreatureUuid(creatureCardUuid))
            return;
        CreatureCard creatureCard = initiator.GetCreatureByUuid(creatureCardUuid);
        if (creatureCard == null)
            return;

        InvokeOnUndeclareAttackerClientRpc(initiator.PlayerId, targetId, creatureCard);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void InvokeOnUndeclareAttackerClientRpc(ulong initiatorId, ulong targetId, CreatureCard card) {
        EventBus.Instance.InvokeOnUndelcareAttacker(new UndeclareAttackerEventArgs(initiatorId, targetId, card));
    }

    private void UndeclareDefender(object sender, CombatFieldCardSelectEventArgs args) {
        if (args.CombatFieldUI != this)
            return;
        if (args.CardUI == null)
            return;

        UndeclareDefenderServerRpc(targetPlayerId, args.CardUI.CardUuid.ToString());
    }

    [Rpc(SendTo.Server)]
    private void UndeclareDefenderServerRpc(ulong targetId, FixedString128Bytes creatureCardUuidStr) {
        if (targetId == duelManager.GetCurrentPlayerTurn().PlayerId)
            return;
        if (!combatStateManager.CurrentState.CanDeclareDefenders())
            return;
        MatchPlayer target = duelManager.GetPlayerById(targetId);
        Guid creatureCardUuid = Guid.Parse(creatureCardUuidStr.ToString());
        if (!target.ContainsCreatureUuid(creatureCardUuid))
            return;
        CreatureCard defender = target.GetCreatureByUuid(creatureCardUuid);
        if (defender == null)
            return;

        InvokeOnUndeclareDefenderClientRpc(duelManager.GetCurrentPlayerTurn().PlayerId, targetId, defender);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void InvokeOnUndeclareDefenderClientRpc(ulong initiatorId, ulong targetId, CreatureCard defender) {
        EventBus.Instance.InvokeOnUndeclareDefender(new UndeclareDefenderEventArgs(initiatorId, targetId, defender));
    }

    public void ReleaseCreatureCards(ulong initiatorId, ulong targetId) {
        EventBus.Instance.InvokeOnReleaseCombatCreatures(new ReleaseCombatCreaturesEventArgs(initiatorId, combatFieldUI.Attackers));
        EventBus.Instance.InvokeOnReleaseCombatCreatures(new ReleaseCombatCreaturesEventArgs(targetId, combatFieldUI.Defenders));
        combatFieldUI.ClearCreatures();
    }

    public bool ContainsAttacker(Guid uuid) {
        return combatFieldUI.ContainsAttacker(uuid);
    }

    public bool ContainsDefender(Guid uuid) {
        return combatFieldUI.ContainsDefender(uuid);
    }
}