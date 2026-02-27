using System;
using UnityEngine;

public class CombatPhase : DuelState {
    public event EventHandler<PlayerEventArgs> OnCombatPhase;
    public event EventHandler<PlayerEventArgs> OnCombatPhaseFinished;

    private DuelStateManager stateManager;
    private CombatManager combatManager;

    public CombatPhase(DuelStateManager stateManager, CombatManager combatManager) {
        this.stateManager = stateManager;
        this.combatManager = combatManager;
    }

    public void EnterState() {
        Debug.Log("Entered Combat Phase");
        OnCombatPhase?.Invoke(this, new PlayerEventArgs(stateManager.DuelManager.GetCurrentPlayerTurn()));
        if(stateManager.DuelManager.IsActivePlayerTurn())
            EventBus.OnActionButtonPressed += ProcessCombat;
        else
            ProcessCombat();
    }

    public void UpdateState() {

    }

    private void ProcessCombat(object sender, EventArgs args) {
        EventBus.OnActionButtonPressed -= ProcessCombat;
        ProcessCombat();
    }

    private void ProcessCombat() {
        combatManager.ProcessCombat();
        OnCombatPhaseFinished?.Invoke(this, new PlayerEventArgs(stateManager.DuelManager.GetCurrentPlayerTurn()));
        stateManager.SwitchState(stateManager.SecondMainPhase);
    }
}
