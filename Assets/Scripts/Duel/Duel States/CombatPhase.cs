using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CombatPhase : NetworkBehaviour, DuelState {
    public event EventHandler<PlayerEventArgs> OnCombatPhase;
    public event EventHandler<PlayerEventArgs> OnCombatPhaseFinished;

    [SerializeField] private DuelStateManager stateManager;
    [SerializeField] private CombatStateManager combatStateManager;
    [SerializeField] private CombatManager combatManager;

    public void EnterState() {
        Debug.Log("Entered Combat Phase");
        OnCombatPhase?.Invoke(this, new PlayerEventArgs(stateManager.DuelManager.GetCurrentPlayerTurn()));
        // TODO: Start CombatStateManager
    }

    public void UpdateState() { }

    [Rpc(SendTo.ClientsAndHost)]
    private void ProcessCombatClientRpc() {
        combatManager.ProcessCombat();
        OnCombatPhaseFinished?.Invoke(this, new PlayerEventArgs(stateManager.DuelManager.GetCurrentPlayerTurn()));
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SwitchToSecondMainPhaseClientRpc() {
        stateManager.SwitchState(stateManager.SecondMainPhase);
    }

    public bool CanPlaySetupCards() {
        return combatStateManager.CurrentState.CanPlaySetupCards();
    }

    public bool CanPlaySpellCards() {
        return combatStateManager.CurrentState.CanPlaySpellCards();
    }

    public bool CanDeclareCombatants() {
        return combatStateManager.CurrentState.CanDeclareCombatants();
    }
}

