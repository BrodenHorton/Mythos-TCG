public class MainPhase : DuelState {

    public void EnterState(DuelStateManager stateManager) {
        stateManager.SwitchState(stateManager.EndPhase);
    }
}
