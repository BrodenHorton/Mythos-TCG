using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerCombatFieldUIController : CombatFieldUIController {
    [SerializeField] private PlayerCombatFieldUI combatFieldUI;

    protected override void Start() {
        base.Start();
        combatFieldUI.OnSelectFieldCard += SelectUndeclareAttacker;
        combatFieldUI.OnSelectFieldCard += SelectUndeclareDefender;
    }

    public override void OnNetworkDespawn() {
        combatFieldUI.OnSelectFieldCard -= SelectUndeclareAttacker;
        combatFieldUI.OnSelectFieldCard -= SelectUndeclareDefender;
    }

    public override void Init(MatchPlayer player) {
        target = player;
        combatFieldUI.Init(player.PlayerId);
    }

    public override void AddAttacker(CreatureCard attacker) {
        combatFieldUI.AddAttacker(attacker);
    }

    public override void AddDefender(CreatureCard defender, Guid attackerCardUuid) {
        combatFieldUI.AddDefender(defender, attackerCardUuid);
    }

    public override void RemoveAttacker(CreatureCard attacker) {
        combatFieldUI.RemoveAttacker(attacker.Uuid);
    }

    public override void RemoveDefender(CreatureCard defender) {
        combatFieldUI.RemoveDefender(defender.Uuid);
    }

    public override void ReleaseCreatureCards(DuelistCombat combat) {
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
        if (stateManager.CurrentState != stateManager.CombatPhase)
            return;
        if (stateManager.CombatPhase.CombateState != CombatPhase.CombatState.DeclareAttackers)
            return;
        if (args.CardUI == null)
            return;
        MatchPlayer initiator = duelManager.GetCurrentPlayerTurn();
        if (!initiator.ContainsCreatureUuid(args.CardUI.CardUuid))
            return;
        CreatureCard creatureCard = initiator.GetCreatureByUuid(args.CardUI.CardUuid);
        if (creatureCard == null)
            return;

        UndeclareAttackerServerRpc(duelManager.GetPlayerIndex(initiator), duelManager.GetPlayerIndex(target), creatureCard.Uuid.ToString());
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
        if (target != duelManager.LocalClientPlayer)
            return;
        if (stateManager.CurrentState != stateManager.CombatPhase)
            return;
        if (stateManager.CombatPhase.CombateState != CombatPhase.CombatState.DeclareDefenders)
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
        UndeclareDefenderServerRpc(duelManager.GetPlayerIndex(initiator), duelManager.GetPlayerIndex(target), creatureCard.Uuid.ToString());
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

    public override bool ContainsAttacker(Guid uuid) {
        return combatFieldUI.ContainsAttacker(uuid);
    }

    public override bool ContainsDefender(Guid uuid) {
        return combatFieldUI.ContainsDefender(uuid);
    }
}
