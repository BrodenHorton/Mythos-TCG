using System;
using Unity.Netcode;
using UnityEngine;

public class CombatPhase : NetworkBehaviour, DuelState {
    public event EventHandler<ulong> OnCombatPhase;
    public event EventHandler<PlayerEventArgs> OnCombatPhaseFinished;

    private DuelStateManager stateManager;
    private CombatStateManager combatStateManager;

    private void Start() {
        combatStateManager = FindFirstObjectByType<CombatStateManager>();
        if (combatStateManager == null)
            throw new Exception("Could not find CombatStateManager object");
        stateManager = FindFirstObjectByType<DuelStateManager>();
        if (stateManager == null)
            throw new Exception("Could not find DuelStateManager object");
    }

    public void EnterState() {
        if (!IsServer)
            return;

        InvokeOnCombatPhaseClientRpc(stateManager.DuelManager.GetCurrentPlayerTurn().PlayerId);
        combatStateManager.OutOfCombatState.OnOutOfCombatEntered += SwitchToSecondMainPhase;
        combatStateManager.StartCombatServerRpc();
    }

    public void UpdateState() { }

    [Rpc(SendTo.ClientsAndHost)]
    private void InvokeOnCombatPhaseClientRpc(ulong playerId) {
        Debug.Log("Entered Combat Phase");
        OnCombatPhase?.Invoke(this, playerId);
    }

    private void SwitchToSecondMainPhase(object sender, EventArgs args) {
        combatStateManager.OutOfCombatState.OnOutOfCombatEntered -= SwitchToSecondMainPhase;
        stateManager.SwitchState(stateManager.SecondMainPhase);
    }

    public bool CanPlaySetupCards() {
        return combatStateManager.CurrentState.CanPlaySetupCards();
    }

    public bool CanPlaySpellCards() {
        return combatStateManager.CurrentState.CanPlaySpellCards();
    }
}

