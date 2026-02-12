using UnityEngine;

public class MainPhase : DuelState {
    private DuelStateManager stateManager;

    public MainPhase(DuelStateManager stateManager) {
        this.stateManager = stateManager;
    }

    public void EnterState() {
        Debug.Log("Entered Main Phase");
        stateManager.SwitchState(stateManager.EndPhase);
    }

    public void UpdateState() {

    }
}
