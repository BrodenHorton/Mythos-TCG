using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombatFieldUI : CombatFieldUI {
    public event EventHandler<CombatFieldCardSelectEventArgs> OnSelectFieldCard;

    private Camera cam;

    protected override void Awake() {
        base.Awake();
        PlayerInputActions playerInputActions = GameInputManager.Instance.PlayerInputActions;
        playerInputActions.Player.Select.started += SelectFieldCard;
    }

    private void Start() {
        cam = Camera.main;
    }

    private void SelectFieldCard(InputAction.CallbackContext context) {
        if (!context.started)
            return;
        CreatureFieldCardUI cardUI = CreatureFieldCardRaycastColliderCheck();
        if (cardUI == null)
            return;
        if (!ContainsAttacker(cardUI) && !ContainsDefender(cardUI))
            return;

        OnSelectFieldCard?.Invoke(this, new CombatFieldCardSelectEventArgs(this, cardUI));
    }

    private CreatureFieldCardUI CreatureFieldCardRaycastColliderCheck() {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);
        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        CreatureFieldCardUI cardUI = null;
        foreach (RaycastHit hit in hits) {
            if (hit.collider.GetComponent<CreatureFieldCardCollisionPointer>()) {
                cardUI = hit.collider.GetComponent<CreatureFieldCardCollisionPointer>().CardUI;
                break;
            }
        }

        return cardUI;
    }
}
