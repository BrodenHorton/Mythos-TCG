using System;
using Unity.Netcode;
using UnityEngine;

public class DeclareAttackersState : NetworkBehaviour, CombatState {
    public event EventHandler<ulong> OnStartDeclareAttackers;

    private CombatStateManager combatStateManager;
    private ActionManager actionManager;

    private void Start() {
        if (!IsServer)
            return;

        combatStateManager = FindFirstObjectByType<CombatStateManager>();
        if (combatStateManager == null)
            throw new Exception("Could not find CombatStateManager object");
        actionManager = FindFirstObjectByType<ActionManager>();
        if (actionManager == null)
            throw new Exception("Could not find ActionManager object");
    }

    public void EnterState() {
        if (!IsServer)
            return;

        ulong currentPlayerTurnId = combatStateManager.DuelManager.GetCurrentPlayerTurn().PlayerId;
        actionManager.AddAction(currentPlayerTurnId, SwitchToDeclareDefendersServerRpc, "Commit", "Waiting for Opponent");
        InvokeOnStartDeclareAttackersClientRpc(currentPlayerTurnId);
    }

    public void UpdateState() { }

    [Rpc(SendTo.ClientsAndHost)]
    private void InvokeOnStartDeclareAttackersClientRpc(ulong playerId) {
        OnStartDeclareAttackers?.Invoke(this, playerId);
    }

    [Rpc(SendTo.Server)]
    private void SwitchToDeclareDefendersServerRpc() {
        SwitchToDeclareDefendersClientRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SwitchToDeclareDefendersClientRpc() {
        combatStateManager.SwitchState(combatStateManager.DeclareDefendersState);
    }

    public bool CanPlaySetupCards() {
        return false;
    }

    public bool CanPlaySpellCards() {
        return false;
    }

    public bool CanDeclareAttackers() {
        return true;
    }

    public bool CanDeclareDefenders() {
        return false;
    }
}
