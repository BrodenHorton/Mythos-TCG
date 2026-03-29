using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActionButtonUIController : MonoBehaviour {
    [SerializeField] private ActionButtonUI actionButtonUI;

    private DuelManager duelManager;
    private DuelStateManager stateManager;
    private Camera cam;

    private void Start() {
        duelManager = FindFirstObjectByType<DuelManager>();
        if (duelManager == null)
            throw new Exception("Could not find DuelManager object");
        stateManager = FindFirstObjectByType<DuelStateManager>();
        if (stateManager == null)
            throw new Exception("Could not find DuelStateManager object");

        cam = Camera.main;

        PlayerInputActions playerInputActions = GameInputManager.Instance.PlayerInputActions;
        playerInputActions.Player.Select.performed += ButtonPressedCheck;

        stateManager.FirstMainPhase.OnFirstMainPhase += FirstMainPhaseAction;
        stateManager.CombatPhase.OnCombatPhase += CombatPhaseAction;
        stateManager.SecondMainPhase.OnSecondMainPhase += SecondMainPhaseAction;
        duelManager.OnNextPlayerTurn += SetActionButtonInactive;
    }

    private void OnDestroy() {
        PlayerInputActions playerInputActions = GameInputManager.Instance.PlayerInputActions;
        playerInputActions.Player.Select.performed -= ButtonPressedCheck;

        stateManager.FirstMainPhase.OnFirstMainPhase -= FirstMainPhaseAction;
        stateManager.CombatPhase.OnCombatPhase -= CombatPhaseAction;
        stateManager.SecondMainPhase.OnSecondMainPhase -= SecondMainPhaseAction;
        duelManager.OnNextPlayerTurn -= SetActionButtonInactive;
    }

    private void FirstMainPhaseAction(object sender, PlayerEventArgs args) {
        if (!duelManager.IsLocalClientPlayerTurn())
            return;

        actionButtonUI.SetActiveAction("Combat Phase");
    }

    private void CombatPhaseAction(object sender, PlayerEventArgs args) {
        if (!duelManager.IsLocalClientPlayerTurn())
            return;

        actionButtonUI.SetActiveAction("Next");
    }

    private void SecondMainPhaseAction(object sender, PlayerEventArgs args) {
        if (!duelManager.IsLocalClientPlayerTurn())
            return;

        actionButtonUI.SetActiveAction("End");
    }

    private void SetActionButtonInactive(object sender, NextPlayerTurnEventArgs args) {
        if(actionButtonUI.IsActive)
            actionButtonUI.SetInactive();
    }

    private void ButtonPressedCheck(InputAction.CallbackContext context) {
        if (!context.performed)
            return;
        if (!duelManager.IsLocalClientPlayerTurn())
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