public class StartPhase : DuelState {
    private DuelStateManager stateManager;

    public StartPhase(DuelStateManager stateManager) {
        this.stateManager = stateManager;
    }

    public void EnterState() {
        stateManager.SwitchState(stateManager.MainPhase);
    }

    public void UpdateState() {

    }
}
