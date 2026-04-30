using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActionButtonUIController : MonoBehaviour {
    [SerializeField] private ActionButtonUI actionButtonUI;

    private DuelManager duelManager;
    private DuelStateManager stateManager;
    private CombatStateManager combatStateManager;
    private ActionManager actionManager;
    private Camera cam;

    private void Start() {
        duelManager = FindFirstObjectByType<DuelManager>();
        if (duelManager == null)
            throw new Exception("Could not find DuelManager object");
        stateManager = FindFirstObjectByType<DuelStateManager>();
        if (stateManager == null)
            throw new Exception("Could not find DuelStateManager object");
        combatStateManager = FindFirstObjectByType<CombatStateManager>();
        if (combatStateManager == null)
            throw new Exception("Could not find CombatStateManager object");
        actionManager = FindFirstObjectByType<ActionManager>();
        if (actionManager == null)
            throw new Exception("Could not find ActionManager object");

        cam = Camera.main;

        PlayerInputActions playerInputActions = GameInputManager.Instance.PlayerInputActions;
        playerInputActions.Player.Select.performed += ButtonPressedCheck;

        actionManager.OnCanPerformActionChanged += (sender, args) => {
            UpdateActionButtonForActionFocusPlayer();
        };
        actionManager.OnActionAdded += (sender, args) => {
            UpdateActionButtonForActionFocusPlayer();
        };
        actionManager.OnActionRemoved += (sender, args) => {
            UpdateActionButtonForActionFocusPlayer();
        };
        actionManager.InactiveActionText.OnValueChanged += UpdateInactiveText;
    }

    private void OnDestroy() {
        PlayerInputActions playerInputActions = GameInputManager.Instance.PlayerInputActions;
        playerInputActions.Player.Select.performed -= ButtonPressedCheck;
    }

    private void UpdateActionButtonForActionFocusPlayer() {
        if (actionManager.CanPerformAction) {
            if (actionManager.Actions.Count > 0) {
                actionButtonUI.SetActive(actionManager.Actions.Peek().ActiveActionMessage);
            }
            else {
                actionButtonUI.SetInactive("");
            }
        }
        else {
            actionButtonUI.SetInactive(actionManager.InactiveActionText.Value.ToString());
        }
    }

    private void UpdateInactiveText(FixedString128Bytes oldInactiveActionText, FixedString128Bytes inactiveActionText) {
        if (actionButtonUI.IsActive)
            return;
        if (actionManager.CanPerformAction)
            return;

        actionButtonUI.SetInactive(inactiveActionText.ToString());
    }

    private void ButtonPressedCheck(InputAction.CallbackContext context) {
        if (!context.performed)
            return;
        if (!actionButtonUI.IsActive)
            return;
        if (actionButtonUI != RaycastColliderCheck())
            return;

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