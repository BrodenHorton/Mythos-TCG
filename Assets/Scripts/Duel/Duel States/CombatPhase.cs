using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CombatPhase : NetworkBehaviour, DuelState {
    public event EventHandler<PlayerEventArgs> OnCombatPhase;
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
        Debug.Log("Entered Combat Phase");
        OnCombatPhase?.Invoke(this, new PlayerEventArgs(stateManager.DuelManager.GetCurrentPlayerTurn()));
        if(IsServer) {
            combatStateManager.OutOfCombatState.OnOutOfCombatEntered += SwitchToSecondMainPhase;
            combatStateManager.StartCombatServerRpc();
        }
    }

    public void UpdateState() { }

    private void SwitchToSecondMainPhase(object sender, EventArgs args) {
        SwitchToSecondMainPhaseServerRpc();
    }

    [Rpc(SendTo.Server)]
    private void SwitchToSecondMainPhaseServerRpc() {
        combatStateManager.OutOfCombatState.OnOutOfCombatEntered -= SwitchToSecondMainPhase;
        SwitchToSecondMainPhaseClientRpc();
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
}

