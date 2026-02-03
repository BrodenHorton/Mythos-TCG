using System;
using System.Collections.Generic;
using UnityEngine;

public class DuelUI : MonoBehaviour {
    [SerializeField] private BattlefieldUI battlefieldUI;
    [SerializeField] private List<PlayerUI> playerUIs;

    private void Awake() {
        DuelManager duelManager = FindFirstObjectByType<DuelManager>();
        if (duelManager == null)
            throw new Exception("Could not find DuelManager object");

        
    }
}
