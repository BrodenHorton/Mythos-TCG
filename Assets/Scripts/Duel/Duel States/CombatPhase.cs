using System;
using Unity.Netcode;
using UnityEngine;

public class CombatPhase : NetworkBehaviour, DuelState {
    public event EventHandler<PlayerEventArgs> OnCombatPhase;
    public event EventHandler<PlayerEventArgs> OnCombatPhaseFinished;

    [SerializeField] private DuelStateManager stateManager;
    [SerializeField] private CombatManager combatManager;

    public void EnterState() {
        Debug.Log("Entered Combat Phase");
        OnCombatPhase?.Invoke(this, new PlayerEventArgs(stateManager.DuelManager.GetCurrentPlayerTurn()));
        if(stateManager.DuelManager.IsLocalClientPlayerTurn())
            EventBus.OnActionButtonPressed += ProcessCombat;
    }

    public void UpdateState() { }

    private void ProcessCombat(object sender, EventArgs args) {
        EventBus.OnActionButtonPressed -= ProcessCombat;
        ProcessCombatRpc();
        SwitchToSecondMainPhaseRpc();
    }

    [Rpc(SendTo.Server)]
    private void ProcessCombatRpc() {
        ProcessCombatClientRpc();
    }

    [ClientRpc]
    private void ProcessCombatClientRpc() {
        combatManager.ProcessCombat();
        OnCombatPhaseFinished?.Invoke(this, new PlayerEventArgs(stateManager.DuelManager.GetCurrentPlayerTurn()));
    }

    [Rpc(SendTo.Server)]
    private void SwitchToSecondMainPhaseRpc() {
        SwitchToSecondMainPhaseClientRpc();
    }

    [ClientRpc]
    private void SwitchToSecondMainPhaseClientRpc() {
        stateManager.SwitchState(stateManager.SecondMainPhase);
    }
}
