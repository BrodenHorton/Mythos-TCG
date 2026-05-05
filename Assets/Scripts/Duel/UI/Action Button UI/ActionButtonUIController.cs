using System;
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