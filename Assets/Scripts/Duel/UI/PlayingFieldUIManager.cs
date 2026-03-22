using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayingFieldUIManager : NetworkBehaviour {
    [SerializeField] private List<PlayingFieldUIController> controllers;

    private DuelManager duelManager;
    private Dictionary<ulong, PlayingFieldUIController> controllerByPlayerId;

    private void Start() {
        duelManager = FindFirstObjectByType<DuelManager>();
        if (duelManager == null)
            throw new Exception("Could not find DuelManager object");

        duelManager.OnPlayersInitialization += Init;
    }

    private void Init(object sender, PlayersInitializedEventArgs args) {
        List<MatchPlayer> players = duelManager.Players;
        if (players.Count != controllers.Count)
            throw new Exception("Number of Match Players and PlayingFieldUIControllers does not match. " +
                players.Count + " Match Players and " + controllers.Count + " PlayingFieldUIControllers");

        controllerByPlayerId = new Dictionary<ulong, PlayingFieldUIController>();
        for (int i = 0; i < players.Count; i++) {
            MatchPlayer localClientOffsetPlayer = players[(args.LocalClientPlayerIndex + i) % args.PlayerCount];
            controllers[i].Init(localClientOffsetPlayer);
            controllerByPlayerId.Add(localClientOffsetPlayer.PlayerId, controllers[i]);
        }
    }
}
