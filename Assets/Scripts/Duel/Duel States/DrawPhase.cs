using System;
using UnityEngine;

public class DrawPhase : DuelState {
    public event EventHandler<EventArgs> OnDrawPhase;

    private DuelStateManager stateManager;

    public DrawPhase(DuelStateManager stateManager) {
        this.stateManager = stateManager;
    }

    public void EnterState() {
        Debug.Log("Entered Draw Phase");
        OnDrawPhase?.Invoke(this, EventArgs.Empty);
        DuelManager duelManager = stateManager.DuelManager;
        duelManager.GetCurrentPlayerTurn().CurrentMana = duelManager.GetStartOfTurnManaCount();
        duelManager.GetCurrentPlayerTurn().DrawCard();
        stateManager.SwitchState(stateManager.FirstMainPhase);
    }

    public void UpdateState() {

    }
}
