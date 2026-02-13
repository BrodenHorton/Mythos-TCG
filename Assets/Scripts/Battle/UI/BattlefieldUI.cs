using System;
using System.Collections.Generic;
using UnityEngine;

public class BattlefieldUI : MonoBehaviour {
    [SerializeField] private List<PlayingFieldUI> playingFieldUIs = new List<PlayingFieldUI>();

    private void Awake() {
        if (playingFieldUIs.Count < 2)
            throw new Exception("Must have two or more PlayingFieldUIs");
    }

    private void Start() {
        DuelManager duelManager = FindFirstObjectByType<DuelManager>();
        if (duelManager == null)
            throw new Exception("Could not find DuelManager object");

        List<MatchPlayer> players = duelManager.Players;
        for(int i = 0; i < playingFieldUIs.Count; i++) {
            if (players.Count <= i)
                break;

            playingFieldUIs[i].PlayerUuid = players[i].Uuid;
        }
    }

}
