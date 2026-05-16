using System;
using UnityEngine;

public class ActionButtonUIController : MonoBehaviour {
    [SerializeField] private ActionButtonUI actionButtonUI;

    private ActionManager actionManager;

    private void Start() {
        actionManager = ServiceLocator.Get<ActionManager>();

        actionButtonUI.OnActionButtonPressed += ExecuteAction;
        actionManager.OnActionStateChanged += UpdateActionButton;
    }

    private void ExecuteAction(object sender, EventArgs args) {
        actionManager.ExecuteActionServerRpc();
    }

    private void UpdateActionButton(object sender, ActionStateEventArgs args) {
        if(args.HasActionFocus)
            actionButtonUI.SetActive(args.ActionMessage);
        else
            actionButtonUI.SetInactive(args.ActionMessage);
    }
}