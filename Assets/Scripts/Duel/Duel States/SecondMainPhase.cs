using System;
using Unity.Netcode;
using UnityEngine;

public class SecondMainPhase : NetworkBehaviour, DuelState {
    public event EventHandler<PlayerEventArgs> OnSecondMainPhase;

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
        Debug.Log("Entered Second Main Phase");
        OnSecondMainPhase?.Invoke(this, new PlayerEventArgs(stateManager.DuelManager.GetCurrentPlayerTurn()));
        if (stateManager.DuelManager.IsLocalClientPlayerTurn()) {
            actionManager.AddAction(SwitchToEndPhaseServerRpc, "End", "Waiting for Opponent");
            actionManager.SetCanPerformAction(true);
        }
    }

    public void UpdateState() { }

    [Rpc(SendTo.Server)]
    private void SwitchToEndPhaseServerRpc() {
        SwitchToEndPhaseClientRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SwitchToEndPhaseClientRpc() {
        stateManager.SwitchState(stateManager.EndPhase);
    }

    public bool CanPlaySetupCards() {
        return true;
    }

    public bool CanPlaySpellCards() {
        return true;
    }
}