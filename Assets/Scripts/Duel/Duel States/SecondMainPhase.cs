using System;
using UnityEngine;

public class SecondMainPhase : DuelState {
    public event EventHandler<PlayerEventArgs> OnSecondMainPhase;

    private DuelStateManager stateManager;

    public SecondMainPhase(DuelStateManager stateManager) {
        this.stateManager = stateManager;
    }

    public void EnterState() {
        Debug.Log("Entered Second Main Phase");
        OnSecondMainPhase?.Invoke(this, new PlayerEventArgs(stateManager.DuelManager.GetCurrentPlayerTurn()));
        if (stateManager.DuelManager.IsActivePlayerTurn())
            EventBus.OnActionButtonPressed += NextPhase;
        else
            stateManager.SwitchState(stateManager.EndPhase);
    }

    public void UpdateState() {

    }

    private void NextPhase(object sender, EventArgs args) {
        EventBus.OnActionButtonPressed -= NextPhase;
        stateManager.SwitchState(stateManager.EndPhase);
    }
}