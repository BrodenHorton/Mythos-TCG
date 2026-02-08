using UnityEngine;

public class EndPhase : DuelState {

    public void EnterState(DuelStateManager stateManager) {
        Debug.Log("End of Turn");
        stateManager.DuelManager.NextTurn();
        stateManager.SwitchState(stateManager.DrawPhase);
    }
}