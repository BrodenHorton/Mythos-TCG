using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DuelistUIManager : NetworkBehaviour {
    [SerializeField] private List<DuelistUIController> controllers;

    private Dictionary<ulong, DuelistUIController> controllerByPlayerId;

    private void Start() {
        DuelManager duelManager = FindFirstObjectByType<DuelManager>();
        if (duelManager == null)
            throw new Exception("Could not find DuelManager object");

        duelManager.OnPlayersInitialization += Init;
        EventBus.Instance.OnLifePointsChanged += LifePointsChanged;
        EventBus.Instance.OnManaCountChanged += ManaCountChanged;
        EventBus.Instance.OnCardDrawn += CardDrawn;
        EventBus.Instance.OnCardRemovedFromHand += CardRemovedFromHand;
    }

    private void Init(object sender, PlayersInitializedEventArgs args) {
        if (args.PlayerOrder.Count != controllers.Count)
            throw new Exception("Number of Match Players and DuelistUIControllers does not match. " +
                args.PlayerOrder.Count + " Match Players and " + controllers.Count + " DuelistUIControllers");

        controllerByPlayerId = new Dictionary<ulong, DuelistUIController>();
        for (int i = 0; i < args.PlayerOrder.Count; i++) {
            ulong localClientOffsetPlayerId = args.PlayerOrder[(args.LocalClientPlayerIndex + i) % args.PlayerOrder.Count];
            controllers[i].Init(localClientOffsetPlayerId, args.InitialLifePoints, args.InitialManaCount);
            controllerByPlayerId.Add(localClientOffsetPlayerId, controllers[i]);
        }
    }

    private void LifePointsChanged(object sender, LifePointsChangedEventArgs args) {
        if (controllerByPlayerId[args.PlayerId.PlayerId] == null)
            throw new Exception("Unable to find duelist controller with player Id: " + args.PlayerId.PlayerId);

        controllerByPlayerId[args.PlayerId.PlayerId].SetLifePoints(args.LifePoints);
    }

    private void ManaCountChanged(object sender, ManaChangedEventArgs args) {
        if (controllerByPlayerId[args.PlayerId] == null)
            throw new Exception("Unable to find duelist controller with player Id: " + args.PlayerId);

        controllerByPlayerId[args.PlayerId].SetManaCount(args.CurrentMana);
    }

    private void CardDrawn(object sender, PlayerCardEventArgs<Card> args) {
        if (controllerByPlayerId[args.PlayerId] == null)
            throw new Exception("Unable to find duelist controller with player Id: " + args.PlayerId);

        controllerByPlayerId[args.PlayerId].DrawCard(args.Card);
    }

    private void CardRemovedFromHand(object sender, CardRemovedFromHandEventArgs args) {
        if (controllerByPlayerId[args.Player.PlayerId] == null)
            throw new Exception("Unable to find duelist controller with player Id: " + args.Player.PlayerId);

        controllerByPlayerId[args.Player.PlayerId].RemoveCardFromHand(args.HandIndex);
    }
}
