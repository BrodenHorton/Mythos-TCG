using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CombatFieldUIController : MonoBehaviour {
    [SerializeField] private CombatFieldUI combatFieldUI;

    private MatchPlayer target;
    private DuelManager duelManager;
    private Camera cam;
    private PlayerInputActions playerInputActions;

    private void Awake() {
        duelManager = FindFirstObjectByType<DuelManager>();
        if (duelManager == null)
            throw new Exception("Could not find DuelManager object");

        cam = Camera.main;
        playerInputActions = new PlayerInputActions();
        playerInputActions.Enable();
        playerInputActions.Player.Select.performed += SelectCard;
    }

    private void Start() {
        EventBus.OnDeclareAttacker += AddAttacker;
        EventBus.OnDeclareDefender += AddDefender;
    }

    public void Init(MatchPlayer player) {
        this.target = player;
    }

    public void AddAttacker(object sender, DeclareAttackerEventArgs args) {
        if (target.Uuid != args.Target.Uuid)
            return;

        combatFieldUI.AddAttacker(args.Attacker);
    }

    // TODO: Implement so the defender corresponds to a given attacker
    public void AddDefender(object sender, DeclareDefenderEventArgs args) {
        if (target.Uuid != args.Target.Uuid)
            return;

        combatFieldUI.AddDefender(args.Defender);
    }

    private void SelectCard(InputAction.CallbackContext context) {
        if (!context.performed)
            return;
        if (!duelManager.IsActivePlayerTurn())
            return;
        CreatureFieldCardUI cardUI = RaycastColliderCheck();
        if (cardUI == null)
            return;
        if (!combatFieldUI.ContainsAttacker(cardUI))
            return;
        MatchPlayer initiator = duelManager.GetCurrentPlayerTurn();
        if (!initiator.ContainsCreatureUuid(cardUI.CardUuid))
            return;
        CreatureCard creatureCard = initiator.GetCreatureByUuid(cardUI.CardUuid);
        if (creatureCard == null)
            return;

        combatFieldUI.RemoveCreature(cardUI);
        EventBus.InvokeOnUndelcareAttacker(this, new UndeclareAttackerEventArgs(duelManager.GetCurrentPlayerTurn(), target, creatureCard));
    }

    private CreatureFieldCardUI RaycastColliderCheck() {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);
        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        CreatureFieldCardUI fieldCardUI = null;
        foreach (RaycastHit hit in hits) {
            if (hit.collider.GetComponent<CreatureFieldCardCollisionPointer>()) {
                fieldCardUI = hit.collider.GetComponent<CreatureFieldCardCollisionPointer>().FieldCardUI;
                break;
            }
        }

        return fieldCardUI;
    }

}