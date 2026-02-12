using UnityEngine;
using UnityEngine.InputSystem;

public class DrawPhase : DuelState {
    private DuelStateManager stateManager;
    private PlayerInputActions playerInputActions;

    public DrawPhase(DuelStateManager stateManager) {
        this.stateManager = stateManager;
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Next.performed += NextPhase;
    }

    public void EnterState() {
        Debug.Log("Entered Draw Phase");
        playerInputActions.Enable();
        DuelManager duelManager = stateManager.DuelManager;
        duelManager.DrawCard(duelManager.GetCurrentPlayerTurn());
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
