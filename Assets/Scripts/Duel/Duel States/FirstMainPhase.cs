using System;
using Unity.Netcode;
using UnityEngine;

public class FirstMainPhase : NetworkBehaviour, DuelState {
    public event EventHandler<PlayerEventArgs> OnFirstMainPhase;

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
        Debug.Log("Entered First Main Phase");
        OnFirstMainPhase?.Invoke(this, new PlayerEventArgs(stateManager.DuelManager.GetCurrentPlayerTurn()));
        if (stateManager.DuelManager.IsLocalClientPlayerTurn()) {
            actionManager.AddAction(SwitchToCombatPhaseServerRpc, "Combat", "Waiting for Opponent");
            actionManager.SetCanPerformAction(true);
        }
    }

    public void UpdateState() { }

    [Rpc(SendTo.Server)]
    private void SwitchToCombatPhaseServerRpc() {
        SwitchToCombatPhaseClientRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SwitchToCombatPhaseClientRpc() {
        stateManager.SwitchState(stateManager.CombatPhase);
    }

    public bool CanPlaySetupCards() {
        return true;
    }

    public bool CanPlaySpellCards() {
        return true;
    }
}