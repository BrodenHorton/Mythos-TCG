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

    public void AddCreatureCard(ulong playerId, CreatureFieldCardUI cardUI) {
        if (controllerByPlayerId[playerId] == null)
            throw new Exception("Unable to find playing field UI controller with player Id: " + playerId);

        controllerByPlayerId[playerId].AddCreatureCard(cardUI);
    }

    public CreatureFieldCardUI ReleaseCreature(ulong playerId, Guid cardUuid) {
        if (controllerByPlayerId[playerId] == null)
            throw new Exception("Unable to find playing field UI controller with player Id: " + playerId);
        if(!controllerByPlayerId[playerId].ContainsCreature(cardUuid))
            throw new Exception("Unable to find creature field card with uuid: " + cardUuid);

        return controllerByPlayerId[playerId].ReleaseCreature(cardUuid);
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

public class FieldUIManager : NetworkBehaviour {
    [SerializeField] private PlayingFieldUIManager playingFieldUIManager;
    [SerializeField] private CombatFieldUIManager combatFieldUIManager;

    private void Start() {
        EventBus.Instance.OnDeclareAttackerFinished += DeclareAttacker;
        EventBus.Instance.OnDeclareDefenderFinished += DeclareDefender;
        EventBus.Instance.OnUndeclareAttackerFinished += UndeclareAttacker;
        EventBus.Instance.OnUndeclareDefenderFinished += UndeclareDefender;
        EventBus.Instance.OnCreatureDestroyedFinished += DestroyCreature;
        EventBus.Instance.OnReleaseCombatCreatures += GetCreatureCardsFromCombat;
    }

    private void DeclareAttacker(object sender, CombatCreaturePayloadEventArgs args) {
        CreatureFieldCardUI cardUI =  playingFieldUIManager.ReleaseCreature(args.InitiatorId, args.Card.Uuid);
        combatFieldUIManager.AddAttacker(args.TargetId, cardUI);
    }

    private void DeclareDefender(object sender, CreatureCombatPayloadEventArgs args) {
        CreatureFieldCardUI cardUI = playingFieldUIManager.ReleaseCreature(args.TargetId, args.Defender.Uuid);
        combatFieldUIManager.AddDefender(args.TargetId, args.Attacker.Uuid, cardUI);
    }

    private void UndeclareAttacker(object sender, CombatCreaturePayloadEventArgs args) {
        CreatureFieldCardUI cardUI = combatFieldUIManager.ReleaseAttacker(args.TargetId, args.Card.Uuid);
        playingFieldUIManager.AddCreatureCard(args.InitiatorId, cardUI);
    }

    private void UndeclareDefender(object sender, CombatCreaturePayloadEventArgs args) {
        CreatureFieldCardUI cardUI = combatFieldUIManager.ReleaseDefender(args.TargetId, args.Card.Uuid);
        playingFieldUIManager.AddCreatureCard(args.TargetId, cardUI);
    }
}