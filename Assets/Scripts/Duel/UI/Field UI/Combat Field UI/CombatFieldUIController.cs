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

        FieldCardSelectionManager.Instance.OnSelectCreatureFieldCard += SelectCombatCreature;
    }

    public override void OnNetworkDespawn() {
        FieldCardSelectionManager.Instance.OnSelectCreatureFieldCard -= SelectCombatCreature;
    }

    public void Init(ulong playerId) {
        targetPlayerId = playerId;
        combatFieldUI.Init(playerId);
    }

    public void AddAttacker(CreatureFieldCardUI cardUI) {
        combatFieldUI.AddAttacker(cardUI);
    }

    public void AddDefender(CreatureFieldCardUI defender, Guid attackerCardUuid) {
        combatFieldUI.AddDefender(defender, attackerCardUuid);
    }

    public void RemoveAttacker(Guid cardUuid) {
        combatFieldUI.RemoveAttacker(cardUuid);
    }

    public void RemoveDefender(Guid cardUuid) {
        combatFieldUI.RemoveDefender(cardUuid);
    }

    public CreatureFieldCardUI ReleaseAttacker(Guid cardUuid) {
        return combatFieldUI.ReleaseAttacker(cardUuid);
    }

    public CreatureFieldCardUI ReleaseDefender(Guid cardUuid) {
        return combatFieldUI.ReleaseDefender(cardUuid);
    }

    private void SelectCombatCreature(object sender, FieldCardEventArgs<CreatureFieldCardUI> args) {
        if (args.CardUI == null || (!combatFieldUI.ContainsAttacker(args.CardUI) && !combatFieldUI.ContainsDefender(args.CardUI)))
            return;

        // Setting IsCanceled to true will stop card dragging. Might want to make a separte event args class
        // for this event to make this its own boolean for clarity.
        args.IsCanceled = true;
        SelectCombatCreatureServerRpc(targetPlayerId, args.CardUI.CardUuid.ToString());
    }

    [Rpc(SendTo.Server)]
    private void SelectCombatCreatureServerRpc(ulong targetId, FixedString128Bytes creatureCardUuidStr, RpcParams rpcParams = default) {
        ulong clientId = rpcParams.Receive.SenderClientId;
        if (clientId == duelManager.GetCurrentPlayerTurn().PlayerId)
            UndeclareAttacker(targetId, Guid.Parse(creatureCardUuidStr.ToString()));
        else
            UndeclareDefender(targetId, Guid.Parse(creatureCardUuidStr.ToString()));
    }

    private void UndeclareAttacker(ulong targetId, Guid creatureCardUuid) {
        if (!IsServer)
            throw new Exception("Only the server can call the method UndeclareAttacker");
        if (!combatStateManager.CurrentState.CanDeclareAttackers())
            return;
        MatchPlayer initiator = duelManager.GetCurrentPlayerTurn();
        if (!initiator.ContainsCreatureUuid(creatureCardUuid))
            return;
        CreatureCard creatureCard = initiator.GetCreatureByUuid(creatureCardUuid);
        if (creatureCard == null)
            throw new Exception("Unable to undeclare attacker since attacking creature is null");

        CombatCreatureEventArgs combatCreatureEventArgs = new CombatCreatureEventArgs(initiator.PlayerId, targetId, creatureCard);
        EventBus.Instance.InvokeOnUndelcareAttacker(combatCreatureEventArgs);
        InvokeOnUndeclareAttackerFinishedClientRpc(initiator.PlayerId, targetId, new CreatureCardPayload(creatureCard));
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void InvokeOnUndeclareAttackerFinishedClientRpc(ulong initiatorId, ulong targetId, CreatureCardPayload card) {
        EventBus.Instance.InvokeOnUndelcareAttackerFinished(new CombatCreaturePayloadEventArgs(initiatorId, targetId, card));
    }

    private void UndeclareDefender(ulong targetId, Guid creatureCardUuid) {
        if (!IsServer)
            throw new Exception("Only the server can call the method UndeclareDefender");
        if (targetId == duelManager.GetCurrentPlayerTurn().PlayerId)
            return;
        if (!combatStateManager.CurrentState.CanDeclareDefenders())
            return;
        MatchPlayer target = duelManager.GetPlayerById(targetId);
        if (!target.ContainsCreatureUuid(creatureCardUuid))
            return;
        CreatureCard defender = target.GetCreatureByUuid(creatureCardUuid);
        if (defender == null)
            throw new Exception("Unable to undeclare defender since defending creature is null");

        ulong initiatorId = duelManager.GetCurrentPlayerTurn().PlayerId;
        CombatCreatureEventArgs combatCreatureEventArgs = new CombatCreatureEventArgs(initiatorId, targetId, defender);
        EventBus.Instance.InvokeOnUndeclareDefender(combatCreatureEventArgs);
        InvokeOnUndeclareDefenderFinishedClientRpc(initiatorId, targetId, new CreatureCardPayload(defender));
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void InvokeOnUndeclareDefenderFinishedClientRpc(ulong initiatorId, ulong targetId, CreatureCardPayload defender) {
        EventBus.Instance.InvokeOnUndeclareDefenderFinished(new CombatCreaturePayloadEventArgs(initiatorId, targetId, defender));
    }

    public List<CreatureFieldCardUI> ReleaseAttackers() {
        List<CreatureFieldCardUI> attackers = combatFieldUI.Attackers;
        combatFieldUI.ClearAttackers();
        return attackers;
    }

    public List<CreatureFieldCardUI> ReleaseDefenders() {
        List<CreatureFieldCardUI> defenders = combatFieldUI.Defenders;
        combatFieldUI.ClearDefenders();
        return defenders;
    }

    public bool ContainsAttacker(Guid uuid) {
        return combatFieldUI.ContainsAttacker(uuid);
    }

    public bool ContainsDefender(Guid uuid) {
        return combatFieldUI.ContainsDefender(uuid);
    }
}