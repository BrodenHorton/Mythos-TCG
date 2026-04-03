using System;
using UnityEngine;

public class CombatFieldPlayableArea : MonoBehaviour {
    [SerializeField] private CombatFieldUI combatFieldUI;

    private ulong targetPlayerId;
    private Camera cam;

    private void Start() {
        cam = Camera.main;

        EventBus.OnReleaseCardDragPlayingField += PlayCardOnReleaseDrag;
    }

    private void PlayCardOnReleaseDrag(object sender, ReleaseFieldCardDragPlayingFieldEventArgs args) {
        if (IsHoveringCombatArea(out ulong combatFieldTargetPlayerId))
            EventBus.InvokeOnReleaseCreatureFieldCardOverCombatArea(this, new ReleaseCreatureFieldCardDragOverCombatAreaEventArgs(combatFieldUI, args.CardUI));
    }

    private bool IsHoveringCombatArea(out ulong combatFieldTargetPlayerId) {
        combatFieldTargetPlayerId = 0;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);
        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        foreach (RaycastHit hit in hits) {
            if (hit.collider.GetComponent<CombatFieldCollisionPointer>() != null) {
                CombatFieldUI combatFieldUI = hit.collider.GetComponent<CombatFieldCollisionPointer>().CombatFieldUI;
                if (combatFieldUI.TargetPlayerId != targetPlayerId) {
                    combatFieldTargetPlayerId = combatFieldUI.TargetPlayerId;
                    return true;
                }
            }
        }

        return false;
    }
}