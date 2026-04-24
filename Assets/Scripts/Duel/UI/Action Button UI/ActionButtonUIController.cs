using System;
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

        actionManager.OnActionFocusChanged += UpdateActionButtonOnActionManagerUpdate;
        actionManager.OnActionAdded += UpdateActionButtonOnActionManagerUpdate;
        actionManager.OnActionRemoved += UpdateActionButtonOnActionManagerUpdate;
        actionManager.OnInactiveActionTextChanged += UpdateInactiveText;
    }

    private void OnDestroy() {
        PlayerInputActions playerInputActions = GameInputManager.Instance.PlayerInputActions;
        playerInputActions.Player.Select.performed -= ButtonPressedCheck;
    }

    private void UpdateActionButtonOnActionManagerUpdate(object sender, EventArgs args) {
        UpdateActionButton();
    }

    private void UpdateActionButton() {
        if (actionManager.ActionFocusPlayerIndices.Contains(duelManager.GetLocalClientPlayerIndex())) {
            if (actionManager.Actions.Count > 0)
                SetActionButtonActive(actionManager.Actions.Peek().ActiveActionMessage());
            else
                SetActionButtonInactive("");
        }
        else
            SetActionButtonInactive(actionManager.InactiveActionText);
    }

    private void UpdateInactiveText(object sender, string inactiveActionText) {
        if (actionButtonUI.IsActive)
            return;

        SetActionButtonInactive(inactiveActionText);
    }

    public void SetActionButtonActive(string actionText) {
        actionButtonUI.SetActive(actionText);
    }

    public void SetActionButtonInactive(string actionText) {
        actionButtonUI.SetInactive(actionText);
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