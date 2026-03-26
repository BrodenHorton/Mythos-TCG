using System.Collections.Generic;
using System;
using Unity.Netcode;
using UnityEngine;

public class InitializationPhase : NetworkBehaviour, DuelState {
    public event EventHandler OnInitializationPhase;

    [SerializeField] private DuelStateManager stateManager;

    private int initialHandSize = 40;

    public void EnterState() {
        Debug.Log("Entered Initialization Phase");
        OnInitializationPhase?.Invoke(this, EventArgs.Empty);
        MatchPlayer player = stateManager.DuelManager.LocalClientPlayer;
        player.ShuffleDeck();
        List<MatchPlayer> players = stateManager.DuelManager.Players;
        for(int i = 0; i < players.Count; i++) {
            for (int j = 0; j < initialHandSize; j++)
                players[i].DrawCard();
        }
        stateManager.SwitchState(stateManager.UntapPhase);
    }

    public void UpdateState() {

    }
}