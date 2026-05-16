using System;
using Unity.Netcode;
using UnityEngine;

public class FirstMainPhase : NetworkBehaviour, DuelState {
    public event EventHandler<ulong> OnFirstMainPhase;

    private DuelStateManager stateManager;
    private ActionManager actionManager;

    private void Start() {
        stateManager = FindFirstObjectByType<DuelStateManager>();
        if (stateManager == null)
            throw new Exception("Could not find DuelStateManager object");
        actionManager = FindFirstObjectByType<ActionManager>();
        if (actionManager == null)
            throw new Exception("Could not find ActionManager object");
    }

    public void EnterState() {
        if (!IsServer)
            return;

        ulong currentTurnPlayerId = stateManager.DuelManager.GetCurrentPlayerTurn().PlayerId;
        InvokeOnFirstMainPhaseClientRpc(currentTurnPlayerId);
        actionManager.AddAction(currentTurnPlayerId, SwitchToCombatPhaseServerRpc, "Combat", "Waiting for Opponent");
    }

    public void UpdateState() { }

    [Rpc(SendTo.ClientsAndHost)]
    private void InvokeOnFirstMainPhaseClientRpc(ulong playerId) {
        Debug.Log("Entered First Main Phase");
        OnFirstMainPhase?.Invoke(this, playerId);
    }

    [Rpc(SendTo.Server)]
    private void SwitchToCombatPhaseServerRpc(ulong _) {
        stateManager.SwitchState(stateManager.CombatPhase);
    }

    public bool CanPlaySetupCards() {
        return true;
    }

    public bool CanPlaySpellCards() {
        return true;
    }
}