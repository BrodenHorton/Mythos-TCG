using System;
using UnityEngine;

[RequireComponent(typeof(CombatFieldUI))]
public class CombatFieldAttackerSelection : MonoBehaviour {
    private CombatFieldUI combatFieldUI;
    private DuelStateManager stateManager;
    private Camera cam;

    private void Start() {
        combatFieldUI = GetComponent<PlayerCombatFieldUI>();
        stateManager = FindFirstObjectByType<DuelStateManager>();
        if (stateManager == null)
            throw new Exception("Could not find DuelStateManager object");

        cam = Camera.main;

        EventBus.OnReleaseCardDragPlayingField += SelectAttackerToDefend;
    }

    private void SelectAttackerToDefend(object sender, ReleaseCreatureFieldCardDragEventArgs args) {
        if (stateManager.CurrentState != stateManager.CombatPhase)
            return;
        if (stateManager.CombatPhase.CombateState != CombatPhase.CombatState.DeclareDefenders)
            return;
        if (combatFieldUI.TargetPlayerId != args.PlayingFieldUI.PlayerId)
            return;

        if (IsHoveringCombatFieldAttacker(out CreatureFieldCardUI attacker))
            EventBus.InvokeOnSelectAttackerToDefend(this, new SelectAttackerToDefendEventArgs(combatFieldUI, attacker, args.CardUI));
    }

    private bool IsHoveringCombatFieldAttacker(out CreatureFieldCardUI attacker) {
        attacker = null;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);
        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        foreach (RaycastHit hit in hits) {
            CreatureFieldCardCollisionPointer collisionPointer = hit.collider.GetComponent<CreatureFieldCardCollisionPointer>();
            if (collisionPointer != null && combatFieldUI.ContainsAttacker(collisionPointer.CardUI)) {
                attacker = collisionPointer.CardUI;
                return true;
            }
        }

        return false;
    }
}
