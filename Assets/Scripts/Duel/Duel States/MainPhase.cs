using System;
using UnityEngine;

public class MainPhase : DuelState {
    public event EventHandler<PlayerEventArgs> OnMainPhase;

    private DuelStateManager stateManager;

    public MainPhase(DuelStateManager stateManager) {
        this.stateManager = stateManager;
    }

    public void EnterState() {
        Debug.Log("Entered First Main Phase");
        OnMainPhase?.Invoke(this, new PlayerEventArgs(stateManager.DuelManager.GetCurrentPlayerTurn()));
        EventBus.OnActionButtonPressed += NextPhase;
    }

    public void UpdateState() {

    }

    private void NextPhase(object sender, EventArgs args) {
        EventBus.OnActionButtonPressed -= NextPhase;
        stateManager.SwitchState(stateManager.CombatPhase);
    }
}
