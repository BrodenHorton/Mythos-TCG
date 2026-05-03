using System;
using Unity.Collections;
using UnityEngine;

public class ActionButtonUIController : MonoBehaviour {
    [SerializeField] private ActionButtonUI actionButtonUI;

    private DuelManager duelManager;
    private ActionManager actionManager;

    private void Start() {
        duelManager = FindFirstObjectByType<DuelManager>();
        if (duelManager == null)
            throw new Exception("Could not find DuelManager object");
        actionManager = FindFirstObjectByType<ActionManager>();
        if (actionManager == null)
            throw new Exception("Could not find ActionManager object");

        actionButtonUI.OnActionButtonPressed += ExecuteAction;
        actionManager.OnCanPerformActionChanged += (sender, args) => {
            UpdateActionButtonForActionFocusPlayer();
        };
        actionManager.OnActionAdded += (sender, args) => {
            UpdateActionButtonForActionFocusPlayer();
        };
        actionManager.OnActionRemoved += (sender, args) => {
            UpdateActionButtonForActionFocusPlayer();
        };
        actionManager.OnInactiveActionTextChanged += UpdateInactiveText;
    }

    private void UpdateActionButtonForActionFocusPlayer() {
        if (actionManager.CanPerformAction) {
            if (actionManager.ActionsByPlayerId.Count > 0)
                actionButtonUI.SetActive(actionManager.ActionsByPlayerId.Peek().ActiveActionMessage);
            else
                actionButtonUI.SetInactive("");
        }
        else
            actionButtonUI.SetInactive(actionManager.InactiveActionText.Value.ToString());
    }

    private void UpdateInactiveText(object sender, string inactiveActionText) {
        if (actionButtonUI.IsActive)
            return;
        if (actionManager.CanPerformAction)
            return;

        actionButtonUI.SetInactive(inactiveActionText.ToString());
    }

    private void ExecuteAction(object sender, EventArgs args) {
        actionManager.ExecuteActionServerRpc();
    }
}