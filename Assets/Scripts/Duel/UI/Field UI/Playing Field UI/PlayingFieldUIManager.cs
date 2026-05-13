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
        EventBus.Instance.OnCreatureCardPlayedFromHand += PlayCreatureCard;
        EventBus.Instance.OnDomainCardPlayedFromHand += PlayDomainCard;
        //EventBus.Instance.OnSpellCardPlayedFromHand += PlaySpellCard;
        EventBus.Instance.OnCreatureTapped += TapCreature;
        EventBus.Instance.OnCreatureUntapped += UntapCreature;
        EventBus.Instance.OnDeclareAttacker += RemoveAttacker;
        EventBus.Instance.OnDeclareDefender += RemoveDefender;
        EventBus.Instance.OnUndeclareAttacker += UndeclareAttacker;
        EventBus.Instance.OnUndeclareDefender += UndeclareDefender;
        EventBus.Instance.OnCreatureDamaged += UpdateCreatureFieldCard;
        EventBus.Instance.OnCreatureHealthChanged += UpdateCreatureFieldCard;
        EventBus.Instance.OnCreatureDestroyed += DestroyCreature;
        EventBus.Instance.OnReleaseCombatCreatures += GetCreatureCardsFromCombat;
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
        if (controllerByPlayerId[args.PlayerId] == null)
            throw new Exception("Unable to find playing field UI controller with player Id: " + args.PlayerId);

        controllerByPlayerId[args.PlayerId].PlayCreatureCard(args.Card);
    }

    public void PlayDomainCard(object sender, PlayCardFromHandEventArgs<DomainCard> args) {
        if (controllerByPlayerId[args.PlayerId] == null)
            throw new Exception("Unable to find playing field UI controller with player Id: " + args.PlayerId);

        controllerByPlayerId[args.PlayerId].PlayDomainCard(args.Card);
    }

    public void PlaySpellCard(object sender, PlayCardFromHandEventArgs<SpellCard> args) {
        if (controllerByPlayerId[args.PlayerId] == null)
            throw new Exception("Unable to find playing field UI controller with player Id: " + args.PlayerId);

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
        if (controllerByPlayerId[args.InitiatorId] == null)
            throw new Exception("Unable to find playing field UI controller with player Id: " + args.InitiatorId);

        if (controllerByPlayerId[args.InitiatorId].ContainsCreature(args.Attacker.Uuid))
            controllerByPlayerId[args.InitiatorId].RemoveCreature(args.Attacker);
    }

    private void RemoveDefender(object sender, DeclareDefenderEventArgs args) {
        if (controllerByPlayerId[args.TargetId] == null)
            throw new Exception("Unable to find playing field UI controller with player Id: " + args.TargetId);

        if (controllerByPlayerId[args.TargetId].ContainsCreature(args.Defender.Uuid))
            controllerByPlayerId[args.TargetId].RemoveCreature(args.Defender);
    }

    public void UndeclareAttacker(object sender, UndeclareAttackerEventArgs args) {
        if (controllerByPlayerId[args.InitiatorId] == null)
            throw new Exception("Unable to find playing field UI controller with player Id: " + args.InitiatorId);

        controllerByPlayerId[args.InitiatorId].PlayCreatureCard(args.Attacker);
    }

    public void UndeclareDefender(object sender, UndeclareDefenderEventArgs args) {
        if (controllerByPlayerId[args.TargetId] == null)
            throw new Exception("Unable to find playing field UI controller with player Id: " + args.TargetId);

        controllerByPlayerId[args.TargetId].PlayCreatureCard(args.Defender);
    }

    public void UpdateCreatureFieldCard(object sender, PlayerCardEventArgs<CreatureCard> args) {
        if (controllerByPlayerId[args.PlayerId] == null)
            throw new Exception("Unable to find playing field UI controller with player Id: " + args.PlayerId);

        if (controllerByPlayerId[args.PlayerId].ContainsCreature(args.Card.Uuid))
            controllerByPlayerId[args.PlayerId].UpdateCreatureFieldCard(args.Card);
    }

    public void DestroyCreature(object sender, PlayerCardEventArgs<CreatureCard> args) {
        if (controllerByPlayerId[args.PlayerId] == null)
            throw new Exception("Unable to find playing field UI controller with player Id: " + args.PlayerId);

        if(controllerByPlayerId[args.PlayerId].ContainsCreature(args.Card.Uuid))
            controllerByPlayerId[args.PlayerId].RemoveCreature(args.Card);
    }

    private void GetCreatureCardsFromCombat(object sender, ReleaseCombatCreaturesEventArgs args) {
        if (controllerByPlayerId[args.PlayerId] == null)
            throw new Exception("Unable to find playing field UI controller with player Id: " + args.PlayerId);

        controllerByPlayerId[args.PlayerId].GetCreatureCardsFromCombat(args.Creatures);
    }
}
