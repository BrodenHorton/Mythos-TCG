public class StartPhase : DuelState {

    public void EnterState(DuelStateManager stateManager) {
        stateManager.SwitchState(stateManager.MainPhase);
    }
}
