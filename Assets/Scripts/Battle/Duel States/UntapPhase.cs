using UnityEngine;

public class UntapPhase : DuelState {
    private DuelStateManager stateManager;

    public UntapPhase(DuelStateManager stateManager) {
        this.stateManager = stateManager;
    }

    public void EnterState() {
        Debug.Log("Entered Untap Phase");
        MatchPlayer player = stateManager.DuelManager.GetCurrentPlayerTurn();
        for(int i = 0; i < player.Creatures.Count; i++) {
            if (player.Creatures[i].IsTapped)
                player.Creatures[i].Untap();
        }
        stateManager.SwitchState(stateManager.DrawPhase);
    }

    public void UpdateState() {

    }
}
