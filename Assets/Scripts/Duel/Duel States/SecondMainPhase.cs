using System;
using Unity.Netcode;
using UnityEngine;

public class SecondMainPhase : NetworkBehaviour, DuelState {
    public event EventHandler<PlayerEventArgs> OnSecondMainPhase;

    private DuelStateManager stateManager;

    private void Start() {
        stateManager = FindFirstObjectByType<DuelStateManager>();
        if (stateManager == null)
            throw new Exception("Could not find DuelStateManager object");
    }

    public void EnterState() {
        Debug.Log("Entered Second Main Phase");
        OnSecondMainPhase?.Invoke(this, new PlayerEventArgs(stateManager.DuelManager.GetCurrentPlayerTurn()));
        if (stateManager.DuelManager.IsLocalClientPlayerTurn())
            EventBus.OnActionButtonPressed += NextPhase;
    }

    public void UpdateState() { }

    private void NextPhase(object sender, EventArgs args) {
        EventBus.OnActionButtonPressed -= NextPhase;
        SwitchToEndPhaseRpc();
    }

    [Rpc(SendTo.Server)]
    private void SwitchToEndPhaseRpc() {
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