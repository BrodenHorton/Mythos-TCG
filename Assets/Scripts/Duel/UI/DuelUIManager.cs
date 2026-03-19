using System;
using System.Collections.Generic;
using UnityEngine;

public class DuelUIManager : MonoBehaviour {
    [SerializeField] private List<DuelistUIController> duelistUIControllers;
    [SerializeField] private List<PlayingFieldUIController> playingFieldUIControllers;
    [SerializeField] private List<CombatFieldUIController> combatFieldUIControllers;

    private DuelManager duelManager;
    private Dictionary<ulong, DuelistUIController> duelistUIControllerByPlayerId;
    private Dictionary<ulong, PlayingFieldUIController> playingFieldUIControllerByPlayerId;
    private Dictionary<ulong, CombatFieldUIController> combatFieldUIControllerByPlayerId;

    private void Start() {
        duelManager = FindFirstObjectByType<DuelManager>();
        if (duelManager == null)
            throw new Exception("Could not find DuelManager object");

        duelManager.OnPlayersInitialized += Init;
    }

    private void Init(object sender, EventArgs args) {
        List<MatchPlayer> players = duelManager.Players;
        if (players.Count != duelistUIControllers.Count)
            throw new Exception("Number of Match Players and DuelistUIControllers does not match. " +
                players.Count + " Match Players and " + duelistUIControllers.Count + " DuelistUIControllers");
        if (players.Count != playingFieldUIControllers.Count)
            throw new Exception("Number of Match Players and PlayingFieldUIs does not match. " +
                players.Count + " Match Players and " + playingFieldUIControllers.Count + " PlayingFieldUIs");
        if (players.Count != combatFieldUIControllers.Count)
            throw new Exception("Number of Match Players and CombatFieldUIControllers does not match. " +
                players.Count + " Match Players and " + combatFieldUIControllers.Count + " CombatFieldUIControllers");

        duelistUIControllerByPlayerId = new Dictionary<ulong, DuelistUIController>();
        playingFieldUIControllerByPlayerId = new Dictionary<ulong, PlayingFieldUIController>();
        combatFieldUIControllerByPlayerId = new Dictionary<ulong, CombatFieldUIController>();
        int playerIndex = duelManager.GetPlayerIndex(duelManager.LocalClientPlayer.PlayerId);
        int playerCount = duelManager.GetPlayerCount();
        for (int i = 0; i < players.Count; i++) {
            MatchPlayer localClientOffsetPlayer = players[(playerIndex + i) % playerCount];
            duelistUIControllers[i].Init(localClientOffsetPlayer);
            playingFieldUIControllers[i].Init(localClientOffsetPlayer);
            combatFieldUIControllers[i].Init(localClientOffsetPlayer);
            duelistUIControllerByPlayerId.Add(localClientOffsetPlayer.PlayerId, duelistUIControllers[i]);
            playingFieldUIControllerByPlayerId.Add(localClientOffsetPlayer.PlayerId, playingFieldUIControllers[i]);
            combatFieldUIControllerByPlayerId.Add(localClientOffsetPlayer.PlayerId, combatFieldUIControllers[i]);
        }
    }

    public DuelistUIController GetDuelistUIControllerByPlayerUUid(ulong playerId) {
        return duelistUIControllerByPlayerId[playerId];
    }

    public PlayingFieldUIController GetPlayingFieldUIControllerByPlayerUuid(ulong playerId) {
        return playingFieldUIControllerByPlayerId[playerId];
    }

    public CombatFieldUIController GetCombatUIControllerByPlayerUUid(ulong playerId) {
        return combatFieldUIControllerByPlayerId[playerId];
    }
}
