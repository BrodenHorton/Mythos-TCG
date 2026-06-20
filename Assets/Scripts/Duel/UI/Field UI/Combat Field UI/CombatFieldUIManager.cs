using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CombatFieldUIManager : NetworkBehaviour {
    [SerializeField] private List<CombatFieldUIController> controllers;

    private Dictionary<ulong, CombatFieldUIController> controllerByPlayerId;

    private void Start() {
        DuelManager duelManager = ServiceLocator.Get<DuelManager>();
        CombatManager combatManager = ServiceLocator.Get<CombatManager>();

        duelManager.OnPlayersInitialization += Init;
        EventBus.Instance.OnCreatureDestroyedFinished += RemoveCreature;
    }

    private void Init(object sender, PlayersInitializedEventArgs args) {
        if (args.PlayerOrder.Count != controllers.Count)
            throw new Exception("Number of Match Players and CombatFieldUIControllers does not match. " +
                args.PlayerOrder.Count + " Match Players and " + controllers.Count + " CombatFieldUIControllers");

        controllerByPlayerId = new Dictionary<ulong, CombatFieldUIController>();
        for (int i = 0; i < args.PlayerOrder.Count; i++) {
            ulong localClientOffsetPlayerId = args.PlayerOrder[(args.LocalClientPlayerIndex + i) % args.PlayerOrder.Count];
            controllers[i].Init(localClientOffsetPlayerId);
            controllerByPlayerId.Add(localClientOffsetPlayerId, controllers[i]);
        }
    }

    public void AddAttacker(ulong targetId, CreatureFieldCardUI card) {
        if (controllerByPlayerId[targetId] == null)
            throw new Exception("Unable to find combat field UI controller with player Id: " + targetId);

        controllerByPlayerId[targetId].AddAttacker(card);
    }

    public void AddDefender(ulong targetId, Guid attackerUuid, CreatureFieldCardUI defender) {
        if (controllerByPlayerId[targetId] == null)
            throw new Exception("Unable to find combat field UI controller with player Id: " + targetId);

        controllerByPlayerId[targetId].AddDefender(defender, attackerUuid);
    }

    public CreatureFieldCardUI ReleaseAttacker(ulong targetId, Guid cardUuid) {
        if (controllerByPlayerId[targetId] == null)
            throw new Exception("Unable to find combat field UI controller with player Id: " + targetId);
        if (!controllerByPlayerId[targetId].ContainsAttacker(cardUuid))
            throw new Exception("Unable to find combat field UI controller with creature Uuid: " + cardUuid);

        return controllerByPlayerId[targetId].ReleaseAttacker(cardUuid);
    }

    public List<CreatureFieldCardUI> ReleaseAttackers(ulong targetId) {
        if (controllerByPlayerId[targetId] == null)
            throw new Exception("Unable to find combat field UI controller with player Id: " + targetId);

        return controllerByPlayerId[targetId].ReleaseAttackers();
    }

    public CreatureFieldCardUI ReleaseDefender(ulong targetId, Guid cardUuid) {
        if (controllerByPlayerId[targetId] == null)
            throw new Exception("Unable to find combat field UI controller with player Id: " + targetId);
        if (!controllerByPlayerId[targetId].ContainsDefender(cardUuid))
            throw new Exception("Unable to find combat field UI controller with creature Uuid: " + cardUuid);

        return controllerByPlayerId[targetId].ReleaseDefender(cardUuid);
    }

    public List<CreatureFieldCardUI> ReleaseDefenders(ulong targetId) {
        if (controllerByPlayerId[targetId] == null)
            throw new Exception("Unable to find combat field UI controller with player Id: " + targetId);

        return controllerByPlayerId[targetId].ReleaseDefenders();
    }

    public void RemoveCreature(object sender, PlayerCardPayloadEventArgs<CreatureCardPayload> args) {
        if (controllerByPlayerId[args.PlayerId] == null)
            throw new Exception("Unable to find combat field UI controller with player Id: " + args.PlayerId);

        if (controllerByPlayerId[args.PlayerId].ContainsAttacker(args.CardPayload.Uuid))
            controllerByPlayerId[args.PlayerId].RemoveAttacker(args.CardPayload.Uuid);
        else if (controllerByPlayerId[args.PlayerId].ContainsDefender(args.CardPayload.Uuid))
            controllerByPlayerId[args.PlayerId].RemoveDefender(args.CardPayload.Uuid);
    }
}