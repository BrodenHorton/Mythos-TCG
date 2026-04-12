using System;
using UnityEngine;

public class CombatFieldPlayableArea : MonoBehaviour {
    [SerializeField] private CombatFieldUI combatFieldUI;
    [SerializeField] private Collider playableAreaCollider;
    [SerializeField] private GameObject playableAreaVisual;

    private DuelStateManager stateManager;
    private Camera cam;

    private void Start() {
        stateManager = FindFirstObjectByType<DuelStateManager>();
        if (stateManager == null)
            throw new Exception("Could not find DuelStateManager object");

        cam = Camera.main;
        playableAreaVisual.SetActive(false);

        EventBus.OnStartCardDragPlayingField += ShowPlayableAreaVisual;
        EventBus.OnReleaseCardDragPlayingField += PlayCardOnReleaseDrag;
    }

    private void ShowPlayableAreaVisual(object sender, FieldCardDragEventArgs<CreatureFieldCardUI> args) {
        if (stateManager.CurrentState != stateManager.CombatPhase)
            return;
        if (stateManager.CombatPhase.CombateState != CombatPhase.CombatState.DeclareAttackers)
            return;
        if (combatFieldUI.TargetPlayerId == args.PlayingFieldUI.PlayerId)
            return;

        playableAreaVisual.SetActive(true);
    }

    private void PlayCardOnReleaseDrag(object sender, ReleaseFieldCardDragEventArgs<CreatureFieldCardUI> args) {
        if (stateManager.CurrentState != stateManager.CombatPhase)
            return;
        if (stateManager.CombatPhase.CombateState != CombatPhase.CombatState.DeclareAttackers)
            return;
        if (combatFieldUI.TargetPlayerId == args.PlayingFieldUI.PlayerId)
            return;

        playableAreaVisual.SetActive(false);
        if (IsHoveringCombatArea())
            EventBus.InvokeOnReleaseCreatureFieldCardOverCombatArea(this, new CreatureFieldCardEnteringCombatFieldEventArgs(combatFieldUI, args.CardUI));
    }

    private bool IsHoveringCombatArea() {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);
        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        foreach (RaycastHit hit in hits) {
            if (hit.collider == playableAreaCollider)
                return true;
        }

        return false;
    }
}
