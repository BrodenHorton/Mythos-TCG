using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class CombatFieldUIController : NetworkBehaviour {
    [SerializeField] private CombatFieldUI combatFieldUI;

    private MatchPlayer target;
    private DuelManager duelManager;
    private DuelStateManager stateManager;
    private CombatManager combatManager;
    private Camera cam;
    private PlayerInputActions playerInputActions;

    private void Awake() {
        duelManager = FindFirstObjectByType<DuelManager>();
        if (duelManager == null)
            throw new Exception("Could not find DuelManager object");
        stateManager = FindFirstObjectByType<DuelStateManager>();
        if (stateManager == null)
            throw new Exception("Could not find DuelStateManager object");
        combatManager = FindFirstObjectByType<CombatManager>();
        if (stateManager == null)
            throw new Exception("Could not find CombatManager object");

        cam = Camera.main;
        playerInputActions = new PlayerInputActions();
        playerInputActions.Enable();
        playerInputActions.Player.Select.performed += SelectCard;
    }

    private void Start() {
        EventBus.OnDeclareAttacker += AddAttacker;
        EventBus.OnDeclareDefender += AddDefender;
        EventBus.OnUndeclareAttacker += RemoveAttacker;
        combatManager.OnDuelistCombatFinsihed += ReleaseCreatureCards;
    }

    public void Init(MatchPlayer player) {
        target = player;
    }

    public void AddAttacker(object sender, DeclareAttackerEventArgs args) {
        if (target.PlayerId != args.Target.PlayerId)
            return;

        combatFieldUI.AddAttacker(args.Attacker);
    }

    // TODO: Implement so the defender corresponds to a given attacker
    public void AddDefender(object sender, DeclareDefenderEventArgs args) {
        if (target.PlayerId != args.Target.PlayerId)
            return;

        combatFieldUI.AddDefender(args.Defender);
    }

    private void ReleaseCreatureCards(object sender, DuelistCombatEventArgs args) {
        if (target != args.Combat.Target)
            return;

        EventBus.InvokeOnReleaseCombatCreatures(this, new ReleaseCombatCreaturesEventArgs(
            args.Combat.Initiator,
            combatFieldUI.Attackers));
        EventBus.InvokeOnReleaseCombatCreatures(this, new ReleaseCombatCreaturesEventArgs(
            args.Combat.Target,
            combatFieldUI.Defenders));
        combatFieldUI.ClearCreatures();
    }

    private void SelectCard(InputAction.CallbackContext context) {
        if (!context.performed)
            return;
        if (!duelManager.IsLocalClientPlayerTurn())
            return;
        if (stateManager.CurrentState != stateManager.CombatPhase)
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

        UndeclareAttackerServerRpc(duelManager.GetPlayerIndex(initiator), duelManager.GetPlayerIndex(target), creatureCard.Uuid.ToString());
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

    [Rpc(SendTo.Server)]
    private void UndeclareAttackerServerRpc(int initiatorIndex, int targetIndex, FixedString128Bytes cardUuidStr) {
        UndeclareAttackerClientRpc(initiatorIndex, targetIndex, cardUuidStr);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void UndeclareAttackerClientRpc(int initiatorIndex, int targetIndex, FixedString128Bytes cardUuidStr) {
        Guid cardUuid = Guid.Parse(cardUuidStr.ToString());
        CreatureCard creatureCard = duelManager.Players[initiatorIndex].GetCreatureByUuid(cardUuid);
        EventBus.InvokeOnUndelcareAttacker(this, new UndeclareAttackerEventArgs(duelManager.Players[initiatorIndex], duelManager.Players[targetIndex], creatureCard));
    }

    private void RemoveAttacker(object sender, UndeclareAttackerEventArgs args) {
        if (args.Target.PlayerId != target.PlayerId)
            return;
        if (!combatFieldUI.ContainsAttacker(args.Attacker.Uuid))
            return;

        combatFieldUI.RemoveAttacker(args.Attacker.Uuid);
    }
}