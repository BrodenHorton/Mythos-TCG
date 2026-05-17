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
        EventBus.Instance.OnDeclareAttacker += AddAttacker;
        EventBus.Instance.OnDeclareDefender += AddDefender;
        EventBus.Instance.OnUndeclareAttackerFinished += RemoveAttacker;
        EventBus.Instance.OnUndeclareDefenderFinished += RemoveDefender;
        EventBus.Instance.OnCreatureDamaged += UpdateCreatureFieldCard;
        EventBus.Instance.OnCreatureHealthChanged += UpdateCreatureFieldCard;
        EventBus.Instance.OnCreatureDestroyed += DestroyCreature;
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

    private void AddAttacker(object sender, DeclareAttackerEventArgs args) {
        if (controllerByPlayerId[args.TargetId] == null)
            throw new Exception("Unable to find combat field UI controller with player Id: " + args.TargetId);

        controllerByPlayerId[args.TargetId].AddAttacker(args.Attacker);
    }

    private void AddDefender(object sender, DeclareDefenderEventArgs args) {
        if (controllerByPlayerId[args.TargetId] == null)
            throw new Exception("Unable to find combat field UI controller with player Id: " + args.TargetId);

        controllerByPlayerId[args.TargetId].AddDefender(args.Defender, args.Attacker.Uuid);
    }

    private void RemoveAttacker(object sender, UndeclareAttackerEventArgs args) {
        if (controllerByPlayerId[args.TargetId] == null)
            throw new Exception("Unable to find combat field UI controller with player Id: " + args.TargetId);
        if (!controllerByPlayerId[args.TargetId].ContainsAttacker(args.Attacker.Uuid))
            throw new Exception("Unable to find combat field UI controller with creature Uuid: " + args.Attacker.Uuid);

        if (controllerByPlayerId[args.TargetId].ContainsAttacker(args.Attacker.Uuid))
            controllerByPlayerId[args.TargetId].RemoveAttacker(args.Attacker);
    }

    private void RemoveDefender(object sender, UndeclareDefenderEventArgs args) {
        if (controllerByPlayerId[args.TargetId] == null)
            throw new Exception("Unable to find combat field UI controller with player Id: " + args.TargetId);
        if (!controllerByPlayerId[args.TargetId].ContainsDefender(args.Defender.Uuid))
            throw new Exception("Unable to find combat field UI controller with creature Uuid: " + args.Defender.Uuid);

        if (controllerByPlayerId[args.TargetId].ContainsDefender(args.Defender.Uuid))
            controllerByPlayerId[args.TargetId].RemoveDefender(args.Defender);
    }

    public void UpdateCreatureFieldCard(object sender, PlayerCardEventArgs<CreatureCard> args) {
        TcgLogger.Log("[CombatFieldUIManager] UpdatingCreatureFieldCard after creature damaged");
        if (controllerByPlayerId[args.PlayerId] == null)
            throw new Exception("Unable to find combat field UI controller with player Id: " + args.PlayerId);

        TcgLogger.Log("[CombatFieldUIManager] PlayerId: " + args.PlayerId);
        if (controllerByPlayerId[args.PlayerId].ContainsAttacker(args.Card.Uuid) || controllerByPlayerId[args.PlayerId].ContainsDefender(args.Card.Uuid))
            controllerByPlayerId[args.PlayerId].UpdateCreatureFieldCard(args.Card);
    }

    public void DestroyCreature(object sender, PlayerCardEventArgs<CreatureCard> args) {
        if (controllerByPlayerId[args.PlayerId] == null)
            throw new Exception("Unable to find playing field UI controller with player Id: " + args.PlayerId);

        if (controllerByPlayerId[args.PlayerId].ContainsAttacker(args.Card.Uuid))
            controllerByPlayerId[args.PlayerId].RemoveAttacker(args.Card);
        else if (controllerByPlayerId[args.PlayerId].ContainsDefender(args.Card.Uuid))
            controllerByPlayerId[args.PlayerId].RemoveDefender(args.Card);
    }

    private void ReleaseCreatureCards(object sender, DuelistCombatEventArgs args) {
        if (controllerByPlayerId[args.TargetId] == null)
            throw new Exception("Unable to find combat field UI controller with player Id: " + args.TargetId);

        controllerByPlayerId[args.TargetId].ReleaseCreatureCards(args.InitiatorId, args.TargetId);
    }
}