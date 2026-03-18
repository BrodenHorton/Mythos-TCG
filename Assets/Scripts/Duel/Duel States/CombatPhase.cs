using System;
using Unity.Netcode;
using UnityEngine;

public class CombatPhase : NetworkBehaviour, DuelState {
    public event EventHandler<PlayerEventArgs> OnCombatPhase;
    public event EventHandler<PlayerEventArgs> OnCombatPhaseFinished;

    [SerializeField] private DuelStateManager stateManager;
    [SerializeField] private CombatManager combatManager;

    public void EnterState() {
        Debug.Log("Entered Combat Phase");
        OnCombatPhase?.Invoke(this, new PlayerEventArgs(stateManager.DuelManager.GetCurrentPlayerTurn()));
        if(stateManager.DuelManager.IsLocalClientPlayerTurn())
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
