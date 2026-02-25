using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CombatPhase : DuelState {
    public event EventHandler<EventArgs> OnCombatPhase;
    public event EventHandler<EventArgs> OnCombatPhaseFinished;

    private DuelStateManager stateManager;
    private CombatManager combatManager;
    private PlayerInputActions playerInputActions;

    public CombatPhase(DuelStateManager stateManager, CombatManager combatManager) {
        this.stateManager = stateManager;
        this.combatManager = combatManager;
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Next.performed += ProcessCombat;
    }

    public void EnterState() {
        Debug.Log("Entered First Combat Phase");
        OnCombatPhase?.Invoke(this, EventArgs.Empty);
        playerInputActions.Enable();
    }

    public void UpdateState() {

    }

    private void ProcessCombat(InputAction.CallbackContext context) {
        if (!context.performed)
            return;

        playerInputActions.Player.Disable();
        combatManager.ProcessCombat();
        OnCombatPhaseFinished?.Invoke(this, EventArgs.Empty);

        stateManager.SwitchState(stateManager.EndPhase);
    }
}
