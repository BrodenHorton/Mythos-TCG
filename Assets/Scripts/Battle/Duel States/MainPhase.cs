using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainPhase : DuelState {
    public event EventHandler<EventArgs> OnMainPhase;

    private DuelStateManager stateManager;
    private PlayerInputActions playerInputActions;

    public MainPhase(DuelStateManager stateManager) {
        this.stateManager = stateManager;
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Next.performed += NextPhase;
    }

    public void EnterState() {
        Debug.Log("Entered First Main Phase");
        OnMainPhase?.Invoke(this, EventArgs.Empty);
        playerInputActions.Enable();
    }

    public void UpdateState() {

    }

    private void NextPhase(InputAction.CallbackContext context) {
        if (!context.performed)
            return;

        playerInputActions.Player.Disable();
        stateManager.SwitchState(stateManager.EndPhase);
    }
}
