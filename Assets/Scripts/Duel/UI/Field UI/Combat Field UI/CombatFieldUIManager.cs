using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CombatFieldUIManager : NetworkBehaviour {
    [SerializeField] private List<CombatFieldUIController> controllers;

    private Dictionary<ulong, CombatFieldUIController> controllerByPlayerId;

    private void Start() {
        DuelManager duelManager = FindFirstObjectByType<DuelManager>();
        if (duelManager == null)
            throw new Exception("Could not find DuelManager object");
        CombatManager combatManager = FindFirstObjectByType<CombatManager>();
        if (combatManager == null)
            throw new Exception("Could not find CombatManager object");

        duelManager.OnPlayersInitialization += Init;
        EventBus.Instance.OnDeclareAttackerFinished += AddAttacker;
        EventBus.Instance.OnDeclareDefenderFinished += AddDefender;
        EventBus.Instance.OnUndeclareAttackerFinished += RemoveAttacker;
        EventBus.Instance.OnUndeclareDefenderFinished += RemoveDefender;
        EventBus.Instance.OnCreatureTappedFinished += UpdateCreatureFieldCard;
        EventBus.Instance.OnCreatureUntappedFinished += UpdateCreatureFieldCard;
        EventBus.Instance.OnCreatureDamagedFinished += UpdateCreatureFieldCard;
        EventBus.Instance.OnCreatureHealedFinished += UpdateCreatureFieldCard;
        EventBus.Instance.OnCreatureDestroyedFinished += DestroyCreature;
        combatManager.OnDuelistCombatFinsihed += ReleaseCreatureCards;
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

    private void AddAttacker(object sender, CombatCreaturePayloadEventArgs args) {
        if (controllerByPlayerId[args.TargetId] == null)
            throw new Exception("Unable to find combat field UI controller with player Id: " + args.TargetId);

        controllerByPlayerId[args.TargetId].AddAttacker(args.Card);
    }

    private void AddDefender(object sender, CreatureCombatPayloadEventArgs args) {
        if (controllerByPlayerId[args.TargetId] == null)
            throw new Exception("Unable to find combat field UI controller with player Id: " + args.TargetId);

        controllerByPlayerId[args.TargetId].AddDefender(args.Defender, args.Attacker.Uuid);
    }

    private void RemoveAttacker(object sender, CombatCreaturePayloadEventArgs args) {
        if (controllerByPlayerId[args.TargetId] == null)
            throw new Exception("Unable to find combat field UI controller with player Id: " + args.TargetId);
        if (!controllerByPlayerId[args.TargetId].ContainsAttacker(args.Card.Uuid))
            throw new Exception("Unable to find combat field UI controller with creature Uuid: " + args.Card.Uuid);

        if (controllerByPlayerId[args.TargetId].ContainsAttacker(args.Card.Uuid))
            controllerByPlayerId[args.TargetId].RemoveAttacker(args.Card.Uuid);
    }

    private void RemoveDefender(object sender, CombatCreaturePayloadEventArgs args) {
        if (controllerByPlayerId[args.TargetId] == null)
            throw new Exception("Unable to find combat field UI controller with player Id: " + args.TargetId);
        if (!controllerByPlayerId[args.TargetId].ContainsDefender(args.Card.Uuid))
            throw new Exception("Unable to find combat field UI controller with creature Uuid: " + args.Card.Uuid);

        if (controllerByPlayerId[args.TargetId].ContainsDefender(args.Card.Uuid))
            controllerByPlayerId[args.TargetId].RemoveDefender(args.Card.Uuid);
    }

    // Move event listeners for card update into the FieldCard class
    public void UpdateCreatureFieldCard(object sender, PlayerCardPayloadEventArgs<CreatureCardPayload> args) {
        foreach (CombatFieldUIController controller in controllerByPlayerId.Values) {
            if (controller.ContainsAttacker(args.CardPayload.Uuid) || controller.ContainsDefender(args.CardPayload.Uuid))
                controller.UpdateCreatureFieldCard(args.CardPayload);
        }
    }

    public void DestroyCreature(object sender, PlayerCardPayloadEventArgs<CreatureCardPayload> args) {
        if (controllerByPlayerId[args.PlayerId] == null)
            throw new Exception("Unable to find playing field UI controller with player Id: " + args.PlayerId);

        if (controllerByPlayerId[args.PlayerId].ContainsAttacker(args.CardPayload.Uuid))
            controllerByPlayerId[args.PlayerId].RemoveAttacker(args.CardPayload.Uuid);
        else if (controllerByPlayerId[args.PlayerId].ContainsDefender(args.CardPayload.Uuid))
            controllerByPlayerId[args.PlayerId].RemoveDefender(args.CardPayload.Uuid);
    }

    private void ReleaseCreatureCards(object sender, DuelistCombatEventArgs args) {
        if (controllerByPlayerId[args.TargetId] == null)
            throw new Exception("Unable to find combat field UI controller with player Id: " + args.TargetId);

        controllerByPlayerId[args.TargetId].ReleaseCreatureCards(args.InitiatorId, args.TargetId);
    }
}