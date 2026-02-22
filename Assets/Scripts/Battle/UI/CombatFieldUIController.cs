using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CombatFieldUIController : MonoBehaviour {
    [SerializeField] private CombatFieldUI combatFieldUI;

    private MatchPlayer defender;
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
        this.defender = player;
    }

    public void AddAttacker(object sender, DeclareAttackerEventArgs args) {
        if (defender.Uuid != args.Defender.Uuid)
            return;

        combatFieldUI.AddAttacker(args.Card);
    }

    // TODO: Implement so the defender corresponds to a given attacker
    public void AddDefender(object sender, DeclareDefenderEventArgs args) {
        if (defender.Uuid != args.Defender.Uuid)
            return;

        combatFieldUI.AddDefender(args.Card);
    }

    private void SelectCard(InputAction.CallbackContext context) {
        if (!context.performed)
            return;
        Debug.Log("1");
        if (!duelManager.IsActivePlayerTurn())
            return;
        Debug.Log("2");
        CreatureFieldCardUI cardUI = RaycastColliderCheck();
        if (cardUI == null)
            return;
        Debug.Log("3");
        if (!combatFieldUI.ContainsAttacker(cardUI))
            return;
        Debug.Log("4");
        MatchPlayer attacker = duelManager.GetCurrentPlayerTurn();
        if (!attacker.ContainsCreatureUuid(cardUI.CardUuid))
            return;
        Debug.Log("5");
        CreatureCard creatureCard = attacker.GetCreatureByUuid(cardUI.CardUuid);
        if (creatureCard == null)
            return;

        Debug.Log("6");
        combatFieldUI.RemoveCreature(cardUI);
        EventBus.InvokeOnUndelcareAttacker(this, new UndeclareAttackerEventArgs(duelManager.GetCurrentPlayerTurn(), creatureCard));
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