using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class StartPhase : DuelState {
    public event EventHandler<EventArgs> OnStartPhase;

    private DuelStateManager stateManager;
    private PlayerInputActions playerInputActions;

    public StartPhase(DuelStateManager stateManager) {
        this.stateManager = stateManager;
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Next.performed += NextPhase;
    }

    public void EnterState() {
        Debug.Log("Entered Start Phase");
        OnStartPhase?.Invoke(this, EventArgs.Empty);
        playerInputActions.Enable();
    }

    public void UpdateState() {

    }

    private void NextPhase(InputAction.CallbackContext context) {
        if (!context.performed)
            return;

        playerInputActions.Player.Disable();
        stateManager.SwitchState(stateManager.MainPhase);
    }
}
