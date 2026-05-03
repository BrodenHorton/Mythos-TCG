using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayingFieldUIManager : NetworkBehaviour {
    [SerializeField] private List<PlayingFieldUIController> controllers;

    private Dictionary<ulong, PlayingFieldUIController> controllerByPlayerId;

    private void Start() {
        DuelManager duelManager = FindFirstObjectByType<DuelManager>();
        if (duelManager == null)
            throw new Exception("Could not find DuelManager object");

        duelManager.OnPlayersInitialization += Init;
        EventBus.OnCreatureCardPlayedFromHand += PlayCreatureCard;
        EventBus.OnDomainCardPlayedFromHand += PlayDomainCard;
        //EventBus.OnSpellCardPlayedFromHand += PlaySpellCard;
        EventBus.OnCreatureTapped += TapCreature;
        EventBus.OnCreatureUntapped += UntapCreature;
        EventBus.OnDeclareAttacker += RemoveAttacker;
        EventBus.OnDeclareDefender += RemoveDefender;
        EventBus.OnUndeclareAttacker += UndeclareAttacker;
        EventBus.OnUndeclareDefender += UndeclareDefender;
        EventBus.OnCreatureDamaged += UpdateCreatureFieldCard;
        EventBus.OnCreatureHealthChanged += UpdateCreatureFieldCard;
        EventBus.OnCreatureDestroyed += DestroyCreature;
        EventBus.OnReleaseCombatCreatures += GetCreatureCardsFromCombat;
    }

    private void Init(object sender, PlayersInitializedEventArgs args) {
        if (args.PlayerOrder.Count != controllers.Count)
            throw new Exception("Number of Match Players and PlayingFieldUIControllers does not match. " +
                args.PlayerOrder.Count + " Match Players and " + controllers.Count + " PlayingFieldUIControllers");

        controllerByPlayerId = new Dictionary<ulong, PlayingFieldUIController>();
        for (int i = 0; i < args.PlayerOrder.Count; i++) {
            ulong localClientOffsetPlayerId = args.PlayerOrder[(args.LocalClientPlayerIndex + i) % args.PlayerOrder.Count];
            controllers[i].Init(localClientOffsetPlayerId);
            controllerByPlayerId.Add(localClientOffsetPlayerId, controllers[i]);
        }
    }

    private void PlayCreatureCard(object sender, PlayCardFromHandEventArgs<CreatureCard> args) {
        if (controllerByPlayerId[args.Player.PlayerId] == null)
            throw new Exception("Unable to find playing field UI controller with player Id: " + args.Player.PlayerId);

        controllerByPlayerId[args.Player.PlayerId].PlayCreatureCard(args.Card);
    }

    public void PlayDomainCard(object sender, PlayCardFromHandEventArgs<DomainCard> args) {
        if (controllerByPlayerId[args.Player.PlayerId] == null)
            throw new Exception("Unable to find playing field UI controller with player Id: " + args.Player.PlayerId);

        controllerByPlayerId[args.Player.PlayerId].PlayDomainCard(args.Card);
    }

    public void PlaySpellCard(object sender, PlayCardFromHandEventArgs<SpellCard> args) {
        if (controllerByPlayerId[args.Player.PlayerId] == null)
            throw new Exception("Unable to find playing field UI controller with player Id: " + args.Player.PlayerId);

        //controllerByPlayerId[args.Player.PlayerId].PlaySpellCard(args.Card);
    }

    public void TapCreature(object sender, CreatureCardEventArgs args) {
        foreach(PlayerPlayingFieldUIController controller in controllerByPlayerId.Values) {
            if(controller.ContainsCreature(args.Card.Uuid)) {
                controller.TapCreature(args.Card);
                return;
            }
        }

        throw new Exception("Unable to find playing field UI controller with card uuid: " + args.Card.Uuid);
    }

    public void UntapCreature(object sender, CreatureCardEventArgs args) {
        foreach (PlayerPlayingFieldUIController controller in controllerByPlayerId.Values) {
            if (controller.ContainsCreature(args.Card.Uuid)) {
                controller.UntapCreature(args.Card);
                return;
            }
        }

        throw new Exception("Unable to find playing field UI controller with card uuid: " + args.Card.Uuid);
    }

    private void RemoveAttacker(object sender, DeclareAttackerEventArgs args) {
        if (controllerByPlayerId[args.Initiator.PlayerId] == null)
            throw new Exception("Unable to find playing field UI controller with player Id: " + args.Initiator.PlayerId);

        if (controllerByPlayerId[args.Initiator.PlayerId].ContainsCreature(args.Attacker.Uuid))
            controllerByPlayerId[args.Initiator.PlayerId].RemoveCreature(args.Attacker);
    }

    private void RemoveDefender(object sender, DeclareDefenderEventArgs args) {
        if (controllerByPlayerId[args.Target.PlayerId] == null)
            throw new Exception("Unable to find playing field UI controller with player Id: " + args.Target.PlayerId);

        if (controllerByPlayerId[args.Target.PlayerId].ContainsCreature(args.Defender.Uuid))
            controllerByPlayerId[args.Target.PlayerId].RemoveCreature(args.Defender);
    }

    public void UndeclareAttacker(object sender, UndeclareAttackerEventArgs args) {
        if (controllerByPlayerId[args.Initiator.PlayerId] == null)
            throw new Exception("Unable to find playing field UI controller with player Id: " + args.Initiator.PlayerId);

        controllerByPlayerId[args.Initiator.PlayerId].PlayCreatureCard(args.Attacker);
    }

    public void UndeclareDefender(object sender, UndeclareDefenderEventArgs args) {
        if (controllerByPlayerId[args.Target.PlayerId] == null)
            throw new Exception("Unable to find playing field UI controller with player Id: " + args.Target.PlayerId);

        controllerByPlayerId[args.Target.PlayerId].PlayCreatureCard(args.Defender);
    }

    public void UpdateCreatureFieldCard(object sender, PlayerCardEventArgs<CreatureCard> args) {
        if (controllerByPlayerId[args.Player.PlayerId] == null)
            throw new Exception("Unable to find playing field UI controller with player Id: " + args.Player.PlayerId);

        if (controllerByPlayerId[args.Player.PlayerId].ContainsCreature(args.Card.Uuid))
            controllerByPlayerId[args.Player.PlayerId].UpdateCreatureFieldCard(args.Card);
    }

    public void DestroyCreature(object sender, PlayerCardEventArgs<CreatureCard> args) {
        if (controllerByPlayerId[args.Player.PlayerId] == null)
            throw new Exception("Unable to find playing field UI controller with player Id: " + args.Player.PlayerId);

        if(controllerByPlayerId[args.Player.PlayerId].ContainsCreature(args.Card.Uuid))
            controllerByPlayerId[args.Player.PlayerId].RemoveCreature(args.Card);
    }

    private void GetCreatureCardsFromCombat(object sender, ReleaseCombatCreaturesEventArgs args) {
        if (controllerByPlayerId[args.Player.PlayerId] == null)
            throw new Exception("Unable to find playing field UI controller with player Id: " + args.Player.PlayerId);

        controllerByPlayerId[args.Player.PlayerId].GetCreatureCardsFromCombat(args.Creatures);
    }
}
