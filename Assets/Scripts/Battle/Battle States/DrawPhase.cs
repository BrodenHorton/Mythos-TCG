public class DrawPhase : DuelState {

    public void EnterState(DuelStateManager stateManager) {
        DuelManager duelManager = stateManager.DuelManager;
        duelManager.DrawCard(duelManager.CurrentPlayerTurn);
    }
}
