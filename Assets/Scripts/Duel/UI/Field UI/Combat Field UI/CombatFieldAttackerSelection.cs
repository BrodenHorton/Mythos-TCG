using System;
using UnityEngine;

[RequireComponent(typeof(CombatFieldUI))]
public class CombatFieldAttackerSelection : MonoBehaviour {
    private CombatFieldUI combatFieldUI;
    private DuelStateManager stateManager;
    private CombatStateManager combatStateManager;
    private Camera cam;

    private void Start() {
        combatFieldUI = GetComponent<CombatFieldUI>();
        stateManager = ServiceLocator.Get<DuelStateManager>();
        combatStateManager = ServiceLocator.Get<CombatStateManager>();

        cam = Camera.main;

        EventBus.Instance.OnReleaseCardDragPlayingField += SelectAttackerToDefend;
    }

    private void SelectAttackerToDefend(object sender, ReleaseFieldCardDragEventArgs<CreatureFieldCardUI> args) {
        if (!combatStateManager.CurrentState.CanDeclareDefenders())
            return;
        if (combatFieldUI.TargetPlayerId != args.PlayingFieldUI.PlayerId)
            return;

        if (IsHoveringCombatFieldAttacker(out CreatureFieldCardUI attacker))
            EventBus.Instance.InvokeOnSelectAttackerToDefend(new SelectAttackerToDefendEventArgs(combatFieldUI, attacker, args.CardUI));
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
