using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class DrawPhase : DuelState {
    public event EventHandler<EventArgs> OnDrawPhase;

    private DuelStateManager stateManager;
    private PlayerInputActions playerInputActions;

    public DrawPhase(DuelStateManager stateManager) {
        this.stateManager = stateManager;
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Next.performed += NextPhase;
    }

    public void EnterState() {
        Debug.Log("Entered Draw Phase");
        OnDrawPhase?.Invoke(this, EventArgs.Empty);
        DuelManager duelManager = stateManager.DuelManager;
        duelManager.SetStartOfTurnMana();
        duelManager.DrawCard(duelManager.GetCurrentPlayerTurn());
        playerInputActions.Enable();
    }

    public void UpdateState() {

    }

    private void NextPhase(InputAction.CallbackContext context) {
        if (!context.performed)
            return;

        playerInputActions.Player.Disable();
        stateManager.SwitchState(stateManager.StartPhase);
    }
}
