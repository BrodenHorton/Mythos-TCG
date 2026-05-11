using System;
using UnityEngine;

public class CombatFieldPlayableArea : MonoBehaviour {
    [SerializeField] private CombatFieldUI combatFieldUI;
    [SerializeField] private Collider playableAreaCollider;
    [SerializeField] private GameObject playableAreaVisual;

    private DuelStateManager stateManager;
    private CombatStateManager combatStateManager;
    private Camera cam;

    private void Start() {
        stateManager = FindFirstObjectByType<DuelStateManager>();
        if (stateManager == null)
            throw new Exception("Could not find DuelStateManager object");
        combatStateManager = FindFirstObjectByType<CombatStateManager>();
        if (combatStateManager == null)
            throw new Exception("Could not find CombatStateManager object");

        cam = Camera.main;
        playableAreaVisual.SetActive(false);

        EventBus.Instance.OnStartCardDragPlayingField += ShowPlayableAreaVisual;
        EventBus.Instance.OnReleaseCardDragPlayingField += PlayCardOnReleaseDrag;
    }

    private void ShowPlayableAreaVisual(object sender, FieldCardDragEventArgs<CreatureFieldCardUI> args) {
        if (!combatStateManager.CurrentState.CanDeclareAttackers())
            return;
        if (combatFieldUI.TargetPlayerId == args.PlayingFieldUI.PlayerId)
            return;

        playableAreaVisual.SetActive(true);
    }

    private void PlayCardOnReleaseDrag(object sender, ReleaseFieldCardDragEventArgs<CreatureFieldCardUI> args) {
        if (!combatStateManager.CurrentState.CanDeclareAttackers())
            return;
        if (combatFieldUI.TargetPlayerId == args.PlayingFieldUI.PlayerId)
            return;

        playableAreaVisual.SetActive(false);
        if (IsHoveringCombatArea())
            EventBus.Instance.InvokeOnReleaseCreatureFieldCardOverCombatArea(new CreatureFieldCardEnteringCombatFieldEventArgs(combatFieldUI, args.CardUI));
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
