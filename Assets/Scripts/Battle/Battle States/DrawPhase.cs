using UnityEngine;

public class DrawPhase : DuelState {
    int runCount = 0;

    public void EnterState(DuelStateManager stateManager) {
        Debug.Log("Entered Draw Phase");
        if(runCount < 3) {
            DuelManager duelManager = stateManager.DuelManager;
            duelManager.DrawCard(duelManager.GetCurrentPlayerTurn());
            runCount++;

            stateManager.SwitchState(stateManager.StartPhase);
        }
    }
}
