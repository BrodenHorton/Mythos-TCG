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
        duelManager = ServiceLocator.Get<DuelManager>();
        stateManager = ServiceLocator.Get<DuelStateManager>();
        combatStateManager = ServiceLocator.Get<CombatStateManager>();

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

    public void AddAttacker(CreatureCardPayload attacker) {
        combatFieldUI.AddAttacker(attacker);
    }

    public void AddDefender(CreatureCardPayload defender, Guid attackerCardUuid) {
        combatFieldUI.AddDefender(defender, attackerCardUuid);
    }

    public void RemoveAttacker(Guid cardUuid) {
        combatFieldUI.RemoveAttacker(cardUuid);
    }

    public void RemoveDefender(Guid cardUuid) {
        combatFieldUI.RemoveDefender(cardUuid);
    }

    public void UpdateCreatureFieldCard(CreatureCardPayload card) {
        if (!ContainsAttacker(card.Uuid) && !ContainsDefender(card.Uuid))
            return;

        combatFieldUI.UpdateCreatureFieldCard(card);
    }

    private void UndeclareAttacker(object sender, CombatFieldCardEventArgs<CreatureFieldCardUI> args) {
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

        EventBus.Instance.InvokeOnUndelcareAttacker(new UndeclareAttackerEventArgs(initiator.PlayerId, targetId, creatureCard));
        InvokeOnUndeclareAttackerFinishedClientRpc(initiator.PlayerId, targetId, new CreatureCardPayload(creatureCard));
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void InvokeOnUndeclareAttackerFinishedClientRpc(ulong initiatorId, ulong targetId, CreatureCardPayload card) {
        EventBus.Instance.InvokeOnUndelcareAttackerFinished(new UndeclareAttackerPayloadEventArgs(initiatorId, targetId, card));
    }

    private void UndeclareDefender(object sender, CombatFieldCardEventArgs<CreatureFieldCardUI> args) {
        if (args.CombatFieldUI != combatFieldUI)
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

        ulong initiatorId = duelManager.GetCurrentPlayerTurn().PlayerId;
        EventBus.Instance.InvokeOnUndeclareDefender(new UndeclareDefenderEventArgs(initiatorId, targetId, defender));
        InvokeOnUndeclareDefenderFinishedClientRpc(initiatorId, targetId, new CreatureCardPayload(defender));
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void InvokeOnUndeclareDefenderFinishedClientRpc(ulong initiatorId, ulong targetId, CreatureCardPayload defender) {
        EventBus.Instance.InvokeOnUndeclareDefenderFinished(new UndeclareDefenderPayloadEventArgs(initiatorId, targetId, defender));
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