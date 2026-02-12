using UnityEngine;

public class EndPhase : DuelState {
    private DuelStateManager stateManager;

    public EndPhase(DuelStateManager stateManager) {
        this.stateManager = stateManager;
    }

    public void EnterState() {
        Debug.Log("End of Turn");
        stateManager.DuelManager.NextTurn();
        stateManager.SwitchState(stateManager.DrawPhase);
    }

    public void UpdateState() {

    }
}