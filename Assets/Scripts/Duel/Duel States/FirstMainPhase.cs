using System;
using Unity.Netcode;
using UnityEngine;

public class FirstMainPhase : NetworkBehaviour, DuelState {
    public event EventHandler<PlayerEventArgs> OnFirstMainPhase;

    private DuelStateManager stateManager;

    private void Start() {
        stateManager = FindFirstObjectByType<DuelStateManager>();
        if (stateManager == null)
            throw new Exception("Could not find DuelStateManager object");
    }

    public void EnterState() {
        Debug.Log("Entered First Main Phase");
        OnFirstMainPhase?.Invoke(this, new PlayerEventArgs(stateManager.DuelManager.GetCurrentPlayerTurn()));
        if (stateManager.DuelManager.IsLocalClientPlayerTurn()) {
            TcgLogger.Log("Added switch to combat phase action to action button");
            EventBus.OnActionButtonPressed += NextPhase;
        }
    }

    public void UpdateState() { }

    private void NextPhase(object sender, EventArgs args) {
        TcgLogger.Log("Action button pressed for going to combat phase");
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

    public bool CanPlaySpellCards() {
        return true;
    }
}
