using System;
using UnityEngine;

public class FirstMainPhase : DuelState {
    public event EventHandler<PlayerEventArgs> OnFirstMainPhase;

    private DuelStateManager stateManager;

    public FirstMainPhase(DuelStateManager stateManager) {
        this.stateManager = stateManager;
    }

    public void EnterState() {
        Debug.Log("Entered First Main Phase");
        OnFirstMainPhase?.Invoke(this, new PlayerEventArgs(stateManager.DuelManager.GetCurrentPlayerTurn()));
        if (stateManager.DuelManager.IsActivePlayerTurn())
            EventBus.OnActionButtonPressed += NextPhase;
        else
            stateManager.SwitchState(stateManager.CombatPhase);
    }

    public void UpdateState() {

    }

    private void NextPhase(object sender, EventArgs args) {
        EventBus.OnActionButtonPressed -= NextPhase;
        stateManager.SwitchState(stateManager.CombatPhase);
    }
}
