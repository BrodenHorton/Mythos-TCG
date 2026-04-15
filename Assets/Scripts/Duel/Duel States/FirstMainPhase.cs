using System;
using Unity.Netcode;
using UnityEngine;

public class FirstMainPhase : NetworkBehaviour, DuelState {
    public event EventHandler<PlayerEventArgs> OnFirstMainPhase;

    [SerializeField] private DuelStateManager stateManager;

    public void EnterState() {
        Debug.Log("Entered First Main Phase");
        OnFirstMainPhase?.Invoke(this, new PlayerEventArgs(stateManager.DuelManager.GetCurrentPlayerTurn()));
        if (stateManager.DuelManager.IsLocalClientPlayerTurn())
            EventBus.OnActionButtonPressed += NextPhase;
    }

    public void UpdateState() { }

    private void NextPhase(object sender, EventArgs args) {
        EventBus.OnActionButtonPressed -= NextPhase;
        SwitchToCombatPhaseRpc();
    }

    [Rpc(SendTo.Server)]
    private void SwitchToCombatPhaseRpc() {
        SwitchToCombatPhaseClientRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SwitchToCombatPhaseClientRpc() {
        stateManager.SwitchState(stateManager.CombatPhase);
    }

    public bool CanPlaySetupCards() {
        return true;
    }

    public bool CanDeclareCombatants() {
        return true;
    }
}
