using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class CombatFieldUIController : NetworkBehaviour {
    [SerializeField] private CombatFieldUI combatFieldUI;

    private ulong targetPlayerId;
    private DuelManager duelManager;
    private CombatManager combatManager;

    private void Start() {
        duelManager = ServiceLocator.Get<DuelManager>();
        combatManager = ServiceLocator.Get<CombatManager>();

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
            combatManager.UndeclareAttacker(targetId, Guid.Parse(creatureCardUuidStr.ToString()));
        else
            combatManager.PlayerSelectUndeclareDefender(targetId, Guid.Parse(creatureCardUuidStr.ToString()));
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