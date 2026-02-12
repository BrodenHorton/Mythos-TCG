using System;
using System.Collections.Generic;
using UnityEngine;

public class DuelUI : MonoBehaviour {
    [SerializeField] private BattlefieldUI battlefieldUI;
    [SerializeField] private List<PlayerUI> playerUIs;

    private void Awake() {
        DuelManager duelManager = FindFirstObjectByType<DuelManager>();
        DuelStateManager duelStateManager = FindFirstObjectByType<DuelStateManager>();
        if (duelManager == null)
            throw new Exception("Could not find DuelManager object");
        if (duelStateManager == null)
            throw new Exception("Could not find DuelStateManager object");

        List<MatchPlayer> players = duelManager.Players;
        for (int i = 0; i < playerUIs.Count; i++) {
            if (players.Count <= i)
                break;
            playerUIs[i].PlayerUuid = players[i].Uuid;
        }
    }
}
