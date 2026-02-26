using System;
using UnityEngine;

public class ActionButtonUIController : MonoBehaviour {
    [SerializeField] private ActionButtonUI actionButtonUI;

    private DuelManager duelManager;
    private DuelStateManager stateManager;

    public void Awake() {
        duelManager = FindFirstObjectByType<DuelManager>();
        if (duelManager == null)
            throw new Exception("Could not find DuelManager object");
        stateManager = FindFirstObjectByType<DuelStateManager>();
        if (stateManager == null)
            throw new Exception("Could not find DuelStateManager object");
    }

    public void Start() {
        stateManager.MainPhase.OnMainPhase += MainPhaseAction;
    }

    private void MainPhaseAction(object sender, PlayerEventArgs args) {
        if (!duelManager.IsActivePlayerTurn())
            return;

        actionButtonUI.SetActionText("Enter Combat Phase");
    }
}