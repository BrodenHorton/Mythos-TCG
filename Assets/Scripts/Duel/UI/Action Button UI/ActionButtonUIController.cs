using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActionButtonUIController : MonoBehaviour {
    [SerializeField] private ActionButtonUI actionButtonUI;

    private DuelManager duelManager;
    private DuelStateManager stateManager;
    private CombatStateManager combatStateManager;
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

        cam = Camera.main;

        PlayerInputActions playerInputActions = GameInputManager.Instance.PlayerInputActions;
        playerInputActions.Player.Select.performed += ButtonPressedCheck;

        stateManager.FirstMainPhase.OnFirstMainPhase += FirstMainPhaseAction;
        combatStateManager.DeclareAttackersState.OnStartDeclareAttackers += CombatPhaseDeclareAttackersAction;
        combatStateManager.DeclareDefendersState.OnStartDeclareDefenders += CombatPhaseDeclareDefendersAction;
        combatStateManager.DeclareDefendersState.OnSetDeclareDefeners += CombatPhaseSetDefenders;
        EventBus.OnLocalClientPlayerReadyUp += SetInactiveAfterLocalClientPlayerReadyUp;
        stateManager.SecondMainPhase.OnSecondMainPhase += SecondMainPhaseAction;
        duelManager.OnNextPlayerTurn += SetActionButtonInactiveOpponentsTurn;
    }

    private void OnDestroy() {
        PlayerInputActions playerInputActions = GameInputManager.Instance.PlayerInputActions;
        playerInputActions.Player.Select.performed -= ButtonPressedCheck;

        stateManager.FirstMainPhase.OnFirstMainPhase -= FirstMainPhaseAction;
        combatStateManager.DeclareAttackersState.OnStartDeclareAttackers -= CombatPhaseDeclareAttackersAction;
        combatStateManager.DeclareDefendersState.OnStartDeclareDefenders -= CombatPhaseDeclareDefendersAction;
        combatStateManager.DeclareDefendersState.OnSetDeclareDefeners -= CombatPhaseSetDefenders;
        EventBus.OnLocalClientPlayerReadyUp -= SetInactiveAfterLocalClientPlayerReadyUp;
        stateManager.SecondMainPhase.OnSecondMainPhase -= SecondMainPhaseAction;
        duelManager.OnNextPlayerTurn -= SetActionButtonInactiveOpponentsTurn;
    }

    private void FirstMainPhaseAction(object sender, PlayerEventArgs args) {
        if (!duelManager.IsLocalClientPlayerTurn())
            return;

        actionButtonUI.SetActive("Combat");
    }

    private void CombatPhaseDeclareAttackersAction(object sender, PlayerEventArgs args) {
        if (!duelManager.IsLocalClientPlayerTurn())
            return;

        actionButtonUI.SetActive("Commit");
    }

    private void CombatPhaseDeclareDefendersAction(object sender, PlayerEventArgs args) {
        if (!duelManager.IsLocalClientPlayerTurn())
            return;

        actionButtonUI.SetInactive("Waiting for Opponent");
    }

    private void CombatPhaseSetDefenders(object sender, PlayerEventArgs args) {
        actionButtonUI.SetActive("Commit");
    }

    private void SetInactiveAfterLocalClientPlayerReadyUp(object sender, EventArgs args) {
        actionButtonUI.SetInactive("Waiting for Opponent");
    }

    private void SecondMainPhaseAction(object sender, PlayerEventArgs args) {
        if (!duelManager.IsLocalClientPlayerTurn())
            return;

        actionButtonUI.SetActive("End");
    }

    private void SetActionButtonInactiveOpponentsTurn(object sender, NextPlayerTurnEventArgs args) {
        if(actionButtonUI.IsActive)
            actionButtonUI.SetInactive("Opponents Turn");
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