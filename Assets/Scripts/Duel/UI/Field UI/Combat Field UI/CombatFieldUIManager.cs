using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CombatFieldUIManager : NetworkBehaviour {
    [SerializeField] private List<CombatFieldUIController> controllers;

    private DuelManager duelManager;
    private CombatManager combatManager;
    private Dictionary<ulong, CombatFieldUIController> controllerByPlayerId;

    private void Start() {
        duelManager = FindFirstObjectByType<DuelManager>();
        if (duelManager == null)
            throw new Exception("Could not find DuelManager object");
        combatManager = FindFirstObjectByType<CombatManager>();
        if (combatManager == null)
            throw new Exception("Could not find CombatManager object");

        duelManager.OnPlayersInitialization += Init;
        EventBus.OnDeclareAttacker += AddAttacker;
        EventBus.OnDeclareDefender += AddDefender;
        EventBus.OnUndeclareAttacker += RemoveAttacker;
        EventBus.OnUndeclareDefender += RemoveDefender;
        EventBus.OnCreatureDamaged += UpdateCreatureFieldCard;
        EventBus.OnCreatureDestroyed += DestroyCreature;
        combatManager.OnDuelistCombatFinsihed += ReleaseCreatureCards;
    }

    private void Init(object sender, PlayersInitializedEventArgs args) {
        List<MatchPlayer> players = duelManager.Players;
        if (players.Count != controllers.Count)
            throw new Exception("Number of Match Players and CombatFieldUIControllers does not match. " +
                players.Count + " Match Players and " + controllers.Count + " CombatFieldUIControllers");

        controllerByPlayerId = new Dictionary<ulong, CombatFieldUIController>();
        for (int i = 0; i < players.Count; i++) {
            MatchPlayer localClientOffsetPlayer = players[(args.LocalClientPlayerIndex + i) % args.PlayerCount];
            controllers[i].Init(localClientOffsetPlayer);
            controllerByPlayerId.Add(localClientOffsetPlayer.PlayerId, controllers[i]);
        }
    }

    private void AddAttacker(object sender, DeclareAttackerEventArgs args) {
        if (controllerByPlayerId[args.Target.PlayerId] == null)
            throw new Exception("Unable to find combat field UI controller with player Id: " + args.Target.PlayerId);

        controllerByPlayerId[args.Target.PlayerId].AddAttacker(args.Attacker);
    }

    private void AddDefender(object sender, DeclareDefenderEventArgs args) {
        if (controllerByPlayerId[args.Target.PlayerId] == null)
            throw new Exception("Unable to find combat field UI controller with player Id: " + args.Target.PlayerId);

        controllerByPlayerId[args.Target.PlayerId].AddDefender(args.Defender, args.Attacker.Uuid);
    }

    private void RemoveAttacker(object sender, UndeclareAttackerEventArgs args) {
        if (controllerByPlayerId[args.Target.PlayerId] == null)
            throw new Exception("Unable to find combat field UI controller with player Id: " + args.Target.PlayerId);
        if(!controllerByPlayerId[args.Target.PlayerId].ContainsAttacker(args.Attacker.Uuid))
            throw new Exception("Unable to find combat field UI controller with creature Uuid: " + args.Attacker.Uuid);

        controllerByPlayerId[args.Target.PlayerId].RemoveAttacker(args.Attacker);
    }

    private void RemoveDefender(object sender, UndeclareDefenderEventArgs args) {
        if (controllerByPlayerId[args.Target.PlayerId] == null)
            throw new Exception("Unable to find combat field UI controller with player Id: " + args.Target.PlayerId);
        if (!controllerByPlayerId[args.Target.PlayerId].ContainsDefender(args.Defender.Uuid))
            throw new Exception("Unable to find combat field UI controller with creature Uuid: " + args.Defender.Uuid);

        controllerByPlayerId[args.Target.PlayerId].RemoveDefender(args.Defender);
    }

    public void UpdateCreatureFieldCard(object sender, PlayerCreatureCardEventArgs args) {
        if (controllerByPlayerId[args.Player.PlayerId] == null)
            throw new Exception("Unable to find combat field UI controller with player Id: " + args.Player.PlayerId);

        controllerByPlayerId[args.Player.PlayerId].UpdateCreatureFieldCard(args.Card);
    }

    public void DestroyCreature(object sender, PlayerCreatureCardEventArgs args) {
        if (controllerByPlayerId[args.Player.PlayerId] == null)
            throw new Exception("Unable to find playing field UI controller with player Id: " + args.Player.PlayerId);

        // TODO: Change this to check for both attackers and defenders to be removed
        if (controllerByPlayerId[args.Player.PlayerId].ContainsDefender(args.Card.Uuid))
            controllerByPlayerId[args.Player.PlayerId].RemoveDefender(args.Card);
    }

    private void ReleaseCreatureCards(object sender, DuelistCombatEventArgs args) {
        if (controllerByPlayerId[args.Combat.Target.PlayerId] == null)
            throw new Exception("Unable to find combat field UI controller with player Id: " + args.Combat.Target.PlayerId);

        controllerByPlayerId[args.Combat.Target.PlayerId].ReleaseCreatureCards(args.Combat);
    }
}