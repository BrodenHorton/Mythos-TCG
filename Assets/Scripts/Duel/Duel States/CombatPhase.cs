using System;
using Unity.Netcode;
using UnityEngine;

public class CombatPhase : NetworkBehaviour, DuelState {
    public event EventHandler<ulong> OnCombatPhaseEntered;
    public event EventHandler<ulong> OnCombatPhaseEnteredFinished;
    public event EventHandler<PlayerEventArgs> OnCombatPhaseEnded;

    private DuelStateManager stateManager;
    private CombatStateManager combatStateManager;

    private void Start() {
        if (!IsServer)
            return;

        combatStateManager = ServiceLocator.Get<CombatStateManager>();
        stateManager = ServiceLocator.Get<DuelStateManager>();
    }

    public void EnterState() {
        if (!IsServer)
            return;

        ulong currentPlayerId = stateManager.DuelManager.GetCurrentPlayerTurn().PlayerId;
        InvokeOnCombatPhaseEnteredClientRpc(currentPlayerId);
        OnCombatPhaseEnteredFinished?.Invoke(this, currentPlayerId);
        combatStateManager.OutOfCombatState.OnOutOfCombatStateEntered += SwitchToSecondMainPhase;
        combatStateManager.StartCombat();
    }

    public void UpdateState() { }

    [Rpc(SendTo.ClientsAndHost)]
    private void InvokeOnCombatPhaseEnteredClientRpc(ulong playerId) {
        Debug.Log("Entered Combat Phase");
        OnCombatPhaseEntered?.Invoke(this, playerId);
    }

    private void SwitchToSecondMainPhase(object sender, EventArgs args) {
        if (!IsServer)
            return;

        combatStateManager.OutOfCombatState.OnOutOfCombatStateEntered -= SwitchToSecondMainPhase;
        stateManager.SwitchState(stateManager.SecondMainPhase);
    }

    public bool CanPlaySetupCards() {
        return combatStateManager.CurrentState.CanPlaySetupCards();
    }

    public bool CanPlaySpellCards() {
        return combatStateManager.CurrentState.CanPlaySpellCards();
    }
}

