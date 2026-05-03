using System;
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

        combatFieldUI.OnSelectFieldCard += SelectUndeclareAttacker;
        combatFieldUI.OnSelectFieldCard += SelectUndeclareDefender;
    }

    public override void OnNetworkDespawn() {
        combatFieldUI.OnSelectFieldCard -= SelectUndeclareAttacker;
        combatFieldUI.OnSelectFieldCard -= SelectUndeclareDefender;
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

    public void ReleaseCreatureCards(DuelistCombat combat) {
        EventBus.InvokeOnReleaseCombatCreatures(this, new ReleaseCombatCreaturesEventArgs(
            combat.Initiator,
            combatFieldUI.Attackers));
        EventBus.InvokeOnReleaseCombatCreatures(this, new ReleaseCombatCreaturesEventArgs(
            combat.Target,
            combatFieldUI.Defenders));
        combatFieldUI.ClearCreatures();
    }

    private void SelectUndeclareAttacker(object sender, CombatFieldCardSelectEventArgs args) {
        if (!duelManager.IsLocalClientPlayerTurn())
            return;
        if (!combatStateManager.CurrentState.CanDeclareAttackers())
            return;
        if (args.CardUI == null)
            return;
        MatchPlayer initiator = duelManager.GetCurrentPlayerTurn();
        if (!initiator.ContainsCreatureUuid(args.CardUI.CardUuid))
            return;
        CreatureCard creatureCard = initiator.GetCreatureByUuid(args.CardUI.CardUuid);
        if (creatureCard == null)
            return;

        UndeclareAttackerServerRpc(duelManager.GetPlayerIndex(initiator), duelManager.GetPlayerIndex(targetPlayerId), creatureCard.Uuid.ToString());
    }

    [Rpc(SendTo.Server)]
    private void UndeclareAttackerServerRpc(int initiatorIndex, int targetIndex, FixedString128Bytes cardUuidStr) {
        UndeclareAttackerClientRpc(initiatorIndex, targetIndex, cardUuidStr);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void UndeclareAttackerClientRpc(int initiatorIndex, int targetIndex, FixedString128Bytes cardUuidStr) {
        Guid cardUuid = Guid.Parse(cardUuidStr.ToString());
        CreatureCard creatureCard = duelManager.Players[initiatorIndex].GetCreatureByUuid(cardUuid);
        EventBus.InvokeOnUndelcareAttacker(this, new UndeclareAttackerEventArgs(duelManager.Players[initiatorIndex], duelManager.Players[targetIndex], creatureCard));
    }

    private void SelectUndeclareDefender(object sender, CombatFieldCardSelectEventArgs args) {
        if (duelManager.IsLocalClientPlayerTurn())
            return;
        if (targetPlayerId != duelManager.LocalClientPlayer)
            return;
        if (!combatStateManager.CurrentState.CanDeclareDefenders())
            return;
        if (args.CardUI == null)
            return;
        MatchPlayer localClientPlayer = duelManager.LocalClientPlayer;
        if (!localClientPlayer.ContainsCreatureUuid(args.CardUI.CardUuid))
            return;
        CreatureCard creatureCard = localClientPlayer.GetCreatureByUuid(args.CardUI.CardUuid);
        if (creatureCard == null)
            return;

        MatchPlayer initiator = duelManager.GetCurrentPlayerTurn();
        UndeclareDefenderServerRpc(duelManager.GetPlayerIndex(initiator), duelManager.GetPlayerIndex(targetPlayerId), creatureCard.Uuid.ToString());
    }

    [Rpc(SendTo.Server)]
    private void UndeclareDefenderServerRpc(int initiatorIndex, int targetIndex, FixedString128Bytes cardUuidStr) {
        UndeclareDefenderClientRpc(initiatorIndex, targetIndex, cardUuidStr);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void UndeclareDefenderClientRpc(int initiatorIndex, int targetIndex, FixedString128Bytes cardUuidStr) {
        Guid cardUuid = Guid.Parse(cardUuidStr.ToString());
        CreatureCard creatureCard = duelManager.Players[targetIndex].GetCreatureByUuid(cardUuid);
        EventBus.InvokeOnUndeclareDefender(this, new UndeclareDefenderEventArgs(duelManager.Players[initiatorIndex], duelManager.Players[targetIndex], creatureCard));
    }

    public bool ContainsAttacker(Guid uuid) {
        return combatFieldUI.ContainsAttacker(uuid);
    }

    public bool ContainsDefender(Guid uuid) {
        return combatFieldUI.ContainsDefender(uuid);
    }
}