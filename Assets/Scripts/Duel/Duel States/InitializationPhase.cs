using System.Collections.Generic;
using System;
using Unity.Netcode;
using UnityEngine;

public class InitializationPhase : NetworkBehaviour, DuelState {
    public static readonly int INITIAL_HAND_SIZE = 5;

    public event EventHandler OnInitializationPhase;

    private DuelStateManager stateManager;
    private ActionManager actionManager;

    private void Start() {
        stateManager = FindFirstObjectByType<DuelStateManager>();
        if (stateManager == null)
            throw new Exception("Could not find DuelStateManager object");
        actionManager = FindFirstObjectByType<ActionManager>();
        if (stateManager == null)
            throw new Exception("Could not find ActionManager object");
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

        if(IsServer) {
            actionManager.SetActionFocusPlayerIndicesClientRpc(0);
            SwitchToUntapPhaseClientRpc();
        }
    }

    public void UpdateState() { }

    [Rpc(SendTo.ClientsAndHost)]
    private void SwitchToUntapPhaseClientRpc() {
        stateManager.SwitchState(stateManager.UntapPhase);
    }

    public bool CanPlaySetupCards() {
        return false;
    }

    public bool CanPlaySpellCards() {
        return false;
    }
}