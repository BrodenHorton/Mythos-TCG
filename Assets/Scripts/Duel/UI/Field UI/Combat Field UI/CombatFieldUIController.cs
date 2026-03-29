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
    private Camera cam;

    private void Start() {
        duelManager = FindFirstObjectByType<DuelManager>();
        if (duelManager == null)
            throw new Exception("Could not find DuelManager object");
        stateManager = FindFirstObjectByType<DuelStateManager>();
        if (stateManager == null)
            throw new Exception("Could not find DuelStateManager object");

        cam = Camera.main;

        PlayerInputActions playerInputActions = GameInputManager.Instance.PlayerInputActions;
        playerInputActions.Player.Select.performed += SelectCard;
    }

    public override void OnNetworkDespawn() {
        PlayerInputActions playerInputActions = GameInputManager.Instance.PlayerInputActions;
        playerInputActions.Player.Select.performed -= SelectCard;
    }

    public void Init(MatchPlayer player) {
        target = player;
    }

    public void AddAttacker(CreatureCard attacker) {
        combatFieldUI.AddAttacker(attacker);
    }

    // TODO: Implement so the defender corresponds to a given attacker
    public void AddDefender(CreatureCard defender) {
        combatFieldUI.AddDefender(defender);
    }

    public void RemoveAttacker(CreatureCard attacker) {
        combatFieldUI.RemoveAttacker(attacker.Uuid);
    }

    public void ReleaseCreatureCards(DuelistCombat combat) {
        EventBus.InvokeOnReleaseCombatCreatures(this, new ReleaseCombatCreaturesEventArgs(
            combat.Initiator,
            combatFieldUI.Attackers));
        EventBus.InvokeOnReleaseCombatCreatures(this, new ReleaseCombatCreaturesEventArgs(
            combat.Target,
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

    public bool ContainsAttacker(Guid uuid) {
        return combatFieldUI.ContainsAttacker(uuid);
    }
}