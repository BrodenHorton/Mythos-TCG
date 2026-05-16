using System;
using Unity.Netcode;
using UnityEngine;

public class SecondMainPhase : NetworkBehaviour, DuelState {
    public event EventHandler<ulong> OnSecondMainPhase;

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
        InvokeOnSecondMainPhaseClientRpc(currentTurnPlayerId);
        actionManager.AddAction(currentTurnPlayerId, SwitchToEndPhaseServerRpc, "End", "Waiting for Opponent");
    }

    public void UpdateState() { }

    [Rpc(SendTo.ClientsAndHost)]
    private void InvokeOnSecondMainPhaseClientRpc(ulong playerId) {
        Debug.Log("Entered Second Main Phase");
        OnSecondMainPhase?.Invoke(this, playerId);
    }

    [Rpc(SendTo.Server)]
    private void SwitchToEndPhaseServerRpc(ulong _) {
        stateManager.SwitchState(stateManager.EndPhase);
    }

    public bool CanPlaySetupCards() {
        return true;
    }

    public bool CanPlaySpellCards() {
        return true;
    }
}