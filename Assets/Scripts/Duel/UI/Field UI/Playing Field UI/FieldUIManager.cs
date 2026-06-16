using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class FieldUIManager : NetworkBehaviour {
    [SerializeField] private PlayingFieldUIManager playingFieldUIManager;
    [SerializeField] private CombatFieldUIManager combatFieldUIManager;
    private CombatManager combatManager;

    private void Start() {
        combatManager = ServiceLocator.Get<CombatManager>();

        EventBus.Instance.OnDeclareAttackerFinished += DeclareAttacker;
        EventBus.Instance.OnDeclareDefenderFinished += DeclareDefender;
        EventBus.Instance.OnUndeclareAttackerFinished += UndeclareAttacker;
        EventBus.Instance.OnUndeclareDefenderFinished += UndeclareDefender;
        EventBus.Instance.OnCreatureDestroyedFinished += DestroyCreature;
        combatManager.OnDuelistCombatFinsihed += ReleaseCombatCreatures;
    }

    private void DeclareAttacker(object sender, CombatCreaturePayloadEventArgs args) {
        CreatureFieldCardUI cardUI =  playingFieldUIManager.ReleaseCreature(args.InitiatorId, args.Card.Uuid);
        combatFieldUIManager.AddAttacker(args.TargetId, cardUI);
    }

    private void DeclareDefender(object sender, CreatureCombatPayloadEventArgs args) {
        CreatureFieldCardUI cardUI = playingFieldUIManager.ReleaseCreature(args.TargetId, args.Defender.Uuid);
        combatFieldUIManager.AddDefender(args.TargetId, args.Attacker.Uuid, cardUI);
    }

    private void UndeclareAttacker(object sender, CombatCreaturePayloadEventArgs args) {
        CreatureFieldCardUI cardUI = combatFieldUIManager.ReleaseAttacker(args.TargetId, args.Card.Uuid);
        playingFieldUIManager.AddCreatureCard(args.InitiatorId, cardUI);
    }

    private void UndeclareDefender(object sender, CombatCreaturePayloadEventArgs args) {
        CreatureFieldCardUI cardUI = combatFieldUIManager.ReleaseDefender(args.TargetId, args.Card.Uuid);
        playingFieldUIManager.AddCreatureCard(args.TargetId, cardUI);
    }

    private void DestroyCreature(object sender, PlayerCardPayloadEventArgs<CreatureCardPayload> args) {
        playingFieldUIManager.DestroyCreature(args.PlayerId, args.CardPayload.Uuid);
        combatFieldUIManager.DestroyCreature(args.PlayerId, args.CardPayload.Uuid);
    }

    private void ReleaseCombatCreatures(object sender, DuelistCombatEventArgs args) {
        List<CreatureFieldCardUI> attackers = combatFieldUIManager.ReleaseAttackers(args.TargetId);
        List<CreatureFieldCardUI> defenders = combatFieldUIManager.ReleaseDefenders(args.TargetId);
        playingFieldUIManager.AddCreatureCards(args.InitiatorId, attackers);
        playingFieldUIManager.AddCreatureCards(args.TargetId, defenders);
    }
}

// TODO: Finish class
public class FieldCardSelectionManager : MonoBehaviour {
    public event EventHandler<> OnSelectCreatureFieldCard;
    public event EventHandler<> OnInspectCreatureFieldCard;

    private Camera cam;

    private void Start() {
        cam = Camera.main;

        PlayerInputActions playerInputActions = GameInputManager.Instance.PlayerInputActions;
        playerInputActions.Player.Select.started += SelectFieldCard;
        // TODO: Create Inspect action map
        playerInputActions.Player.Select.started += InspectFieldCard;
    }

    private void OnDestroy() {
        PlayerInputActions playerInputActions = GameInputManager.Instance.PlayerInputActions;
        playerInputActions.Player.Select.started -= SelectFieldCard;
        playerInputActions.Player.Select.started -= InspectFieldCard;
    }

    private void SelectFieldCard(InputAction.CallbackContext context) {
        if (!context.started)
            return;
        CreatureFieldCardUI cardUI = CreatureFieldCardRaycastColliderCheck();
        if (cardUI == null)
            return;
        if (!cardUI.IsSelectable)
            return;


    }

    private void InspectFieldCard(InputAction.CallbackContext context) {

    }

    // TODO: Figure out how to create a fieldCardCollisionPointer that encompasses all other collision pointers
    private FieldCardUI FieldCardRaycastColliderCheck() {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);
        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        FieldCardUI cardUI = null;
        foreach (RaycastHit hit in hits) {
            if (hit.collider.GetComponent<FieldCardCollisionPointer>()) {
                cardUI = hit.collider.GetComponent<FieldCardCollisionPointer>().CardUI;
                break;
            }
        }

        return cardUI;
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