using System;
using System.Collections.Generic;
using UnityEngine;

public class DuelUIManager : MonoBehaviour {
    [SerializeField] private List<DuelistUIController> duelistUIControllers;
    [SerializeField] private List<PlayingFieldUIController> playingFieldUIControllers;
    [SerializeField] private List<CombatFieldUIController> combatFieldUIControllers;

    private Dictionary<Guid, DuelistUIController> duelistUIControllerByPlayerUuid;
    private Dictionary<Guid, PlayingFieldUIController> playingFieldUIControllerByPlayerUuid;
    private Dictionary<Guid, CombatFieldUIController> combatFieldUIControllerByPlayerUuid;

    private void Awake() {
        DuelManager duelManager = FindFirstObjectByType<DuelManager>();
        if (duelManager == null)
            throw new Exception("Could not find DuelManager object");
        List<MatchPlayer> players = duelManager.Players;
        if(players.Count != duelistUIControllers.Count)
            throw new Exception("Number of Match Players and DuelistUIControllers does not match. " + 
                players.Count + " Match Players and " + duelistUIControllers.Count + " DuelistUIControllers");
        if (players.Count != playingFieldUIControllers.Count)
            throw new Exception("Number of Match Players and PlayingFieldUIs does not match. " +
                players.Count + " Match Players and " + playingFieldUIControllers.Count + " PlayingFieldUIs");
        if (players.Count != combatFieldUIControllers.Count)
            throw new Exception("Number of Match Players and CombatFieldUIControllers does not match. " +
                players.Count + " Match Players and " + combatFieldUIControllers.Count + " CombatFieldUIControllers");

        duelistUIControllerByPlayerUuid = new Dictionary<Guid, DuelistUIController>();
        playingFieldUIControllerByPlayerUuid = new Dictionary<Guid, PlayingFieldUIController>();
        combatFieldUIControllerByPlayerUuid = new Dictionary<Guid, CombatFieldUIController>();
        for (int i = 0; i < players.Count; i++) {
            duelistUIControllers[i].Init(players[i]);
            playingFieldUIControllers[i].Init(players[i]);
            combatFieldUIControllers[i].Init(players[i]);
            duelistUIControllerByPlayerUuid.Add(players[i].Uuid, duelistUIControllers[i]);
            playingFieldUIControllerByPlayerUuid.Add(players[i].Uuid, playingFieldUIControllers[i]);
            combatFieldUIControllerByPlayerUuid.Add(players[i].Uuid, combatFieldUIControllers[i]);
        }
    }
    public DuelistUIController GetDuelistUIControllerByPlayerUUid(Guid playerUuid) {
        return duelistUIControllerByPlayerUuid[playerUuid];
    }

    public PlayingFieldUIController GetPlayingFieldUIControllerByPlayerUuid(Guid playerUuid) {
        return playingFieldUIControllerByPlayerUuid[playerUuid];
    }

    public CombatFieldUIController GetCombatUIControllerByPlayerUUid(Guid playerUuid) {
        return combatFieldUIControllerByPlayerUuid[playerUuid];
    }
}
