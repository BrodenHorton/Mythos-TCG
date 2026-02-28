using System;
using UnityEngine;

public class EndPhase : DuelState {
    public EventHandler<PlayerEventArgs> OnEndPhase;

    private DuelStateManager stateManager;

    public EndPhase(DuelStateManager stateManager) {
        this.stateManager = stateManager;
    }

    public void EnterState() {
        Debug.Log("End of Turn");
        OnEndPhase?.Invoke(this, new PlayerEventArgs(stateManager.DuelManager.GetCurrentPlayerTurn()));
        stateManager.DuelManager.NextTurn();
        stateManager.SwitchState(stateManager.UntapPhase);
    }

    public void UpdateState() {

    }
}