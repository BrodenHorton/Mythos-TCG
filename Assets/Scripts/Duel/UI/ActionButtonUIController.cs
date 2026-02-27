using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActionButtonUIController : MonoBehaviour {
    [SerializeField] private ActionButtonUI actionButtonUI;

    private DuelManager duelManager;
    private DuelStateManager stateManager;
    private Camera cam;
    private PlayerInputActions playerInputActions;

    private void Awake() {
        duelManager = FindFirstObjectByType<DuelManager>();
        if (duelManager == null)
            throw new Exception("Could not find DuelManager object");
        stateManager = FindFirstObjectByType<DuelStateManager>();
        if (stateManager == null)
            throw new Exception("Could not find DuelStateManager object");

        cam = Camera.main;
        playerInputActions = new PlayerInputActions();
        playerInputActions.Enable();
        playerInputActions.Player.Select.performed += ButtonPressedCheck;
    }

    private void Start() {
        stateManager.FirstMainPhase.OnFirstMainPhase += FirstMainPhaseAction;
        stateManager.CombatPhase.OnCombatPhase += CombatPhaseAction;
        stateManager.SecondMainPhase.OnSecondMainPhase += SecondMainPhaseAction;
    }

    private void FirstMainPhaseAction(object sender, PlayerEventArgs args) {
        if (!duelManager.IsActivePlayerTurn())
            return;

        actionButtonUI.SetActionText("Combat Phase");
    }

    private void CombatPhaseAction(object sender, PlayerEventArgs args) {
        if (!duelManager.IsActivePlayerTurn())
            return;

        actionButtonUI.SetActionText("Next");
    }

    private void SecondMainPhaseAction(object sender, PlayerEventArgs args) {
        if (!duelManager.IsActivePlayerTurn())
            return;

        actionButtonUI.SetActionText("End");
    }

    private void ButtonPressedCheck(InputAction.CallbackContext context) {
        if (!context.performed)
            return;
        if (!duelManager.IsActivePlayerTurn())
            return;
        if (!actionButtonUI.IsActive)
            return;
        if (actionButtonUI != RaycastColliderCheck())
            return;

        Debug.Log("Action Buttoned Pressed");
        actionButtonUI.Execute();
    }

    private ActionButtonUI RaycastColliderCheck() {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);
        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        ActionButtonUI actionButtonUI = null;
        foreach (RaycastHit hit in hits) {
            if (hit.collider.GetComponent<ActionButtonCollisionPointer>()) {
                actionButtonUI = hit.collider.GetComponent<ActionButtonCollisionPointer>().ActionButtonUI;
                break;
            }
        }

        return actionButtonUI;
    }
}