using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class CombatFieldUIController : NetworkBehaviour {
    [SerializeField] private CombatFieldUI combatFieldUI;

    private ulong targetPlayerId;
    private DuelManager duelManager;
    private CombatManager combatManager;

    private void Start() {
        duelManager = ServiceLocator.Get<DuelManager>();
        combatManager = ServiceLocator.Get<CombatManager>();

        EventBus.Instance.OnSelectCreatureFieldCard += SelectCombatCreature;
        EventBus.Instance.OnStartCreatureFieldCardDrag += ShowPlayableAreaVisualOnFieldCardDrag;
        EventBus.Instance.OnReleaseCreatureFieldCardDrag += ReleaseCreatureCardDragHandler;
    }

    public override void OnNetworkDespawn() {
        EventBus.Instance.OnSelectCreatureFieldCard -= SelectCombatCreature;
        EventBus.Instance.OnStartCreatureFieldCardDrag -= ShowPlayableAreaVisualOnFieldCardDrag;
        EventBus.Instance.OnReleaseCreatureFieldCardDrag -= ReleaseCreatureCardDragHandler;
    }

    public void Init(ulong playerId) {
        targetPlayerId = playerId;
        combatFieldUI.Init(playerId);
    }

    public void AddAttacker(CreatureFieldCardUI cardUI) {
        combatFieldUI.AddAttacker(cardUI);
    }

    public void AddDefender(CreatureFieldCardUI defender, Guid attackerCardUuid) {
        combatFieldUI.AddDefender(defender, attackerCardUuid);
    }

    public void RemoveAttacker(Guid cardUuid) {
        combatFieldUI.RemoveAttacker(cardUuid);
    }

    public void RemoveDefender(Guid cardUuid) {
        combatFieldUI.RemoveDefender(cardUuid);
    }

    public CreatureFieldCardUI ReleaseAttacker(Guid cardUuid) {
        return combatFieldUI.ReleaseAttacker(cardUuid);
    }

    public CreatureFieldCardUI ReleaseDefender(Guid cardUuid) {
        return combatFieldUI.ReleaseDefender(cardUuid);
    }

    public List<CreatureFieldCardUI> ReleaseAttackers() {
        List<CreatureFieldCardUI> attackers = combatFieldUI.Attackers;
        combatFieldUI.ClearAttackers();
        return attackers;
    }

    public List<CreatureFieldCardUI> ReleaseDefenders() {
        List<CreatureFieldCardUI> defenders = combatFieldUI.Defenders;
        combatFieldUI.ClearDefenders();
        return defenders;
    }

    private void SelectCombatCreature(object sender, CardUIEventArgs<CreatureFieldCardUI> args) {
        if (args.CardUI == null || (!combatFieldUI.ContainsAttacker(args.CardUI) && !combatFieldUI.ContainsDefender(args.CardUI)))
            return;

        SelectCombatCreatureServerRpc(targetPlayerId, args.CardUI.CardUuid.ToString());
    }

    [Rpc(SendTo.Server)]
    private void SelectCombatCreatureServerRpc(ulong targetId, FixedString128Bytes creatureCardUuidStr, RpcParams rpcParams = default) {
        ulong clientId = rpcParams.Receive.SenderClientId;
        if (clientId == duelManager.GetCurrentPlayerTurn().PlayerId)
            combatManager.UndeclareAttacker(targetId, Guid.Parse(creatureCardUuidStr.ToString()));
        else
            combatManager.PlayerSelectUndeclareDefender(targetId, Guid.Parse(creatureCardUuidStr.ToString()));
    }

    private void ShowPlayableAreaVisualOnFieldCardDrag(object sender, CardUIEventArgs<CreatureFieldCardUI> args) {
        if (combatFieldUI.TargetPlayerId == args.CardUI.PlayerId)
            return;

        combatFieldUI.ShowPlayableArea();
    }

    // TODO: Move all card drag and release logic to the server since the server has all the context for what should
    // happen when a card is dragged and released.
    private void ReleaseCreatureCardDragHandler(object sender, CardUIEventArgs<CreatureFieldCardUI> args) {
        if (combatFieldUI.TargetPlayerId == args.CardUI.PlayerId) {
            if (combatFieldUI.IsHoveringCombatFieldCreatureCard(out CreatureFieldCardUI hoveredCardUI, args.CardUI)) {
                CreatureReleasedOverCreatureServerRpc(args.CardUI.PlayerId,
                                                      hoveredCardUI.PlayerId,
                                                      args.CardUI.CardUuid.ToString(),
                                                      hoveredCardUI.CardUuid.ToString());
            }
        }
        else {
            if (combatFieldUI.IsHoveringCombatArea())
                EventBus.Instance.InvokeOnReleaseCreatureFieldCardOverCombatArea(new CombatFieldCardEventArgs(combatFieldUI, args.CardUI));
        }
        combatFieldUI.HidePlayableArea();
    }

    [Rpc(SendTo.Server)]
    private void CreatureReleasedOverCreatureServerRpc(ulong heldCardPlayerId,
                                                       ulong hoveredCardPlayerId,
                                                       FixedString128Bytes heldCreatureUuidStr,
                                                       FixedString128Bytes hoveredCreatureUuidStr,
                                                       RpcParams rpcParams = default) {
        MatchPlayer heldCardPlayer = duelManager.GetPlayerById(heldCardPlayerId);
        MatchPlayer hoveredCardPlayer = duelManager.GetPlayerById(hoveredCardPlayerId);
        Guid heldCreatureUuid = Guid.Parse(heldCreatureUuidStr.ToString());
        Guid hoveredCreatureUuid = Guid.Parse(hoveredCreatureUuidStr.ToString());
        CreatureCard heldCreature = heldCardPlayer.GetCreatureByUuid(heldCreatureUuid);
        CreatureCard hoveredCreature = hoveredCardPlayer.GetCreatureByUuid(hoveredCreatureUuid);

        CreatureReleasedOverCreatureEventArgs args = new CreatureReleasedOverCreatureEventArgs(rpcParams.Receive.SenderClientId,
                                                                                               heldCreature,
                                                                                               hoveredCreature);
        EventBus.Instance.InvokeOnCreatureReleasedOverCreature(args);
    }

    public bool ContainsAttacker(Guid uuid) {
        return combatFieldUI.ContainsAttacker(uuid);
    }

    public bool ContainsDefender(Guid uuid) {
        return combatFieldUI.ContainsDefender(uuid);
    }
}