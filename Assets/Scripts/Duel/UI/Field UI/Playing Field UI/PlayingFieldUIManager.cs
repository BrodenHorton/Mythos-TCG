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
        EventBus.Instance.OnCreatureTapped += TapCreature;
        EventBus.Instance.OnCreatureUntapped += UntapCreature;
        EventBus.Instance.OnDeclareAttackerFinished += RemoveAttacker;
        EventBus.Instance.OnDeclareDefenderFinished += RemoveDefender;
        EventBus.Instance.OnUndeclareAttackerFinished += UndeclareAttacker;
        EventBus.Instance.OnUndeclareDefenderFinished += UndeclareDefender;
        EventBus.Instance.OnCreatureDamagedFinished += UpdateCreatureFieldCard;
        EventBus.Instance.OnCreatureHealedFinished += UpdateCreatureFieldCard;
        EventBus.Instance.OnCreatureDestroyedFinished += DestroyCreature;
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

    private void PlayCreatureCard(object sender, PlayerCardPayloadEventArgs<CreatureCardPayload> args) {
        if (controllerByPlayerId[args.PlayerId] == null)
            throw new Exception("Unable to find playing field UI controller with player Id: " + args.PlayerId);

        controllerByPlayerId[args.PlayerId].PlayCreatureCard(args.CardPayload);
    }

    public void PlayDomainCard(object sender, PlayerCardPayloadEventArgs<DomainCardPayload> args) {
        if (controllerByPlayerId[args.PlayerId] == null)
            throw new Exception("Unable to find playing field UI controller with player Id: " + args.PlayerId);

        controllerByPlayerId[args.PlayerId].PlayDomainCard(args.CardPayload);
    }

    public void TapCreature(object sender, CardPayloadEventArgs<CreatureCardPayload> args) {
        foreach(PlayerPlayingFieldUIController controller in controllerByPlayerId.Values) {
            if(controller.ContainsCreature(args.CardPayload.Uuid)) {
                controller.TapCreature(args.CardPayload.Uuid);
                return;
            }
        }

        throw new Exception("Unable to find playing field UI controller with card uuid: " + args.CardPayload.Uuid);
    }

    public void UntapCreature(object sender, CardPayloadEventArgs<CreatureCardPayload> args) {
        foreach (PlayerPlayingFieldUIController controller in controllerByPlayerId.Values) {
            if (controller.ContainsCreature(args.CardPayload.Uuid)) {
                controller.UntapCreature(args.CardPayload.Uuid);
                return;
            }
        }

        throw new Exception("Unable to find playing field UI controller with card uuid: " + args.CardPayload.Uuid);
    }

    private void RemoveAttacker(object sender, DeclareAttackerPayloadEventArgs args) {
        if (controllerByPlayerId[args.InitiatorId] == null)
            throw new Exception("Unable to find playing field UI controller with player Id: " + args.InitiatorId);

        if (controllerByPlayerId[args.InitiatorId].ContainsCreature(args.Attacker.Uuid))
            controllerByPlayerId[args.InitiatorId].RemoveCreature(args.Attacker.Uuid);
    }

    private void RemoveDefender(object sender, DeclareDefenderPayloadEventArgs args) {
        if (controllerByPlayerId[args.TargetId] == null)
            throw new Exception("Unable to find playing field UI controller with player Id: " + args.TargetId);

        if (controllerByPlayerId[args.TargetId].ContainsCreature(args.Defender.Uuid))
            controllerByPlayerId[args.TargetId].RemoveCreature(args.Defender.Uuid);
    }

    public void UndeclareAttacker(object sender, UndeclareAttackerPayloadEventArgs args) {
        if (controllerByPlayerId[args.InitiatorId] == null)
            throw new Exception("Unable to find playing field UI controller with player Id: " + args.InitiatorId);

        controllerByPlayerId[args.InitiatorId].PlayCreatureCard(args.Attacker);
    }

    public void UndeclareDefender(object sender, UndeclareDefenderPayloadEventArgs args) {
        if (controllerByPlayerId[args.TargetId] == null)
            throw new Exception("Unable to find playing field UI controller with player Id: " + args.TargetId);

        controllerByPlayerId[args.TargetId].PlayCreatureCard(args.Defender);
    }

    public void UpdateCreatureFieldCard(object sender, PlayerCardPayloadEventArgs<CreatureCardPayload> args) {
        if (controllerByPlayerId[args.PlayerId] == null)
            throw new Exception("Unable to find playing field UI controller with player Id: " + args.PlayerId);

        if (controllerByPlayerId[args.PlayerId].ContainsCreature(args.CardPayload.Uuid))
            controllerByPlayerId[args.PlayerId].UpdateCreatureFieldCard(args.CardPayload);
    }

    public void DestroyCreature(object sender, PlayerCardPayloadEventArgs<CreatureCardPayload> args) {
        if (controllerByPlayerId[args.PlayerId] == null)
            throw new Exception("Unable to find playing field UI controller with player Id: " + args.PlayerId);

        if(controllerByPlayerId[args.PlayerId].ContainsCreature(args.CardPayload.Uuid))
            controllerByPlayerId[args.PlayerId].RemoveCreature(args.CardPayload.Uuid);
    }

    private void GetCreatureCardsFromCombat(object sender, ReleaseCombatCreaturesEventArgs args) {
        if (controllerByPlayerId[args.PlayerId] == null)
            throw new Exception("Unable to find playing field UI controller with player Id: " + args.PlayerId);

        controllerByPlayerId[args.PlayerId].GetCreatureCardsFromCombat(args.Creatures);
    }
}
