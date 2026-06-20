using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayingFieldUIManager : NetworkBehaviour {
    [SerializeField] private List<PlayingFieldUIController> controllers;

    private Dictionary<ulong, PlayingFieldUIController> controllerByPlayerId;

    private void Start() {
        DuelManager duelManager = ServiceLocator.Get<DuelManager>();

        duelManager.OnPlayersInitialization += Init;
        EventBus.Instance.OnCreatureCardPlayedFromHand += PlayCreatureCard;
        EventBus.Instance.OnDomainCardPlayedFromHand += PlayDomainCard;
        EventBus.Instance.OnCreatureDestroyedFinished += RemoveCreature;
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

    public void AddCreatureCards(ulong playerId, List<CreatureFieldCardUI> creatures) {
        if (controllerByPlayerId[playerId] == null)
            throw new Exception("Unable to find playing field UI controller with player Id: " + playerId);

        controllerByPlayerId[playerId].AddCreatureCards(creatures);
    }

    public CreatureFieldCardUI ReleaseCreature(ulong playerId, Guid cardUuid) {
        if (controllerByPlayerId[playerId] == null)
            throw new Exception("Unable to find playing field UI controller with player Id: " + playerId);
        if(!controllerByPlayerId[playerId].ContainsCreature(cardUuid))
            throw new Exception("Unable to find creature field card with uuid: " + cardUuid);

        return controllerByPlayerId[playerId].ReleaseCreature(cardUuid);
    }

    public void RemoveCreature(object sender, PlayerCardPayloadEventArgs<CreatureCardPayload> args) {
        if (controllerByPlayerId[args.PlayerId] == null)
            throw new Exception("Unable to find playing field UI controller with player Id: " + args.PlayerId);

        if(controllerByPlayerId[args.PlayerId].ContainsCreature(args.CardPayload.Uuid))
            controllerByPlayerId[args.PlayerId].RemoveCreature(args.CardPayload.Uuid);
    }
}
