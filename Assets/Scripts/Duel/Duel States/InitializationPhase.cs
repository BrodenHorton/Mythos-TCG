using System.Collections.Generic;
using System;
using Unity.Netcode;
using UnityEngine;

public class InitializationPhase : NetworkBehaviour, DuelState {
    public static readonly int INITIAL_HAND_SIZE = 5;

    public event EventHandler OnInitializationPhase;

    private DuelStateManager stateManager;

    private void Start() {
        stateManager = FindFirstObjectByType<DuelStateManager>();
        if (stateManager == null)
            throw new Exception("Could not find DuelStateManager object");
    }

    public void EnterState() {
        Debug.Log("Entered Initialization Phase");
        OnInitializationPhase?.Invoke(this, EventArgs.Empty);
        MatchPlayer player = stateManager.DuelManager.LocalClientPlayer;
        player.ShuffleDeck();
        List<MatchPlayer> players = stateManager.DuelManager.Players;
        for(int i = 0; i < players.Count; i++) {
            for (int j = 0; j < INITIAL_HAND_SIZE; j++)
                players[i].DrawCard();
        }
        stateManager.SwitchState(stateManager.UntapPhase);
    }

    public void UpdateState() { }

    public bool CanPlaySetupCards() {
        return false;
    }

    public bool CanPlaySpellCards() {
        return false;
    }
}