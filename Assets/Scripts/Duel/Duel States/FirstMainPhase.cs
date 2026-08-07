using System;
using Unity.Netcode;
using UnityEngine;

public class FirstMainPhase : NetworkBehaviour, DuelState {
    public event EventHandler<ulong> OnFirstMainPhaseEntered;
    public event EventHandler<ulong> OnFirstMainPhaseEnteredFinished;

    private DuelStateManager stateManager;
    private ActionManager actionManager;

    private void Start() {
        stateManager = ServiceLocator.Get<DuelStateManager>();
        actionManager = ServiceLocator.Get<ActionManager>();
    }

    public void EnterState() {
        if (!IsServer)
            return;

        ulong currentTurnPlayerId = stateManager.DuelManager.GetCurrentPlayerTurn().PlayerId;
        InvokeOnFirstMainPhaseEnteredClientRpc(currentTurnPlayerId);
        OnFirstMainPhaseEnteredFinished?.Invoke(this, currentTurnPlayerId);
        actionManager.AddAction(currentTurnPlayerId, SwitchToCombatPhaseServerRpc, "Combat", "Waiting for Opponent");
    }

    public void UpdateState() { }

    [Rpc(SendTo.ClientsAndHost)]
    private void InvokeOnFirstMainPhaseEnteredClientRpc(ulong playerId) {
        Debug.Log("Entered First Main Phase");
        OnFirstMainPhaseEntered?.Invoke(this, playerId);
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