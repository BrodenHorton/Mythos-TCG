using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DuelistUIManager : NetworkBehaviour {
    [SerializeField] private List<DuelistUIController> controllers;

    private DuelManager duelManager;
    private Dictionary<ulong, DuelistUIController> controllerByPlayerId;

    private void Start() {
        duelManager = FindFirstObjectByType<DuelManager>();
        if (duelManager == null)
            throw new Exception("Could not find DuelManager object");

        duelManager.OnPlayersInitialization += Init;
    }

    private void Init(object sender, PlayersInitializedEventArgs args) {
        List<MatchPlayer> players = duelManager.Players;
        if (players.Count != controllers.Count)
            throw new Exception("Number of Match Players and DuelistUIControllers does not match. " +
                players.Count + " Match Players and " + controllers.Count + " DuelistUIControllers");

        controllerByPlayerId = new Dictionary<ulong, DuelistUIController>();
        for (int i = 0; i < players.Count; i++) {
            MatchPlayer localClientOffsetPlayer = players[(args.LocalClientPlayerIndex + i) % args.PlayerCount];
            controllers[i].Init(localClientOffsetPlayer);
            controllerByPlayerId.Add(localClientOffsetPlayer.PlayerId, controllers[i]);
        }
    }
}
