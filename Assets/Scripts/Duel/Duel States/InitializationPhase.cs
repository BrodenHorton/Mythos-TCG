using System.Collections.Generic;
using System;
using Unity.Netcode;
using UnityEngine;

public class InitializationPhase : NetworkBehaviour, DuelState {
    public event EventHandler OnInitializationPhase;

    [SerializeField] private DuelStateManager stateManager;

    private int initialHandSize = 5;

    public void EnterState() {
        Debug.Log("Entered Initialization Phase");
        OnInitializationPhase?.Invoke(this, EventArgs.Empty);
        if(IsServer) {
            PlayerSetup();
            SwitchStateClientRpc();
        }
    }

    public void UpdateState() { }

    [Rpc(SendTo.Server)]
    private void PlayerSetup() {
        List<MatchPlayer> players = stateManager.DuelManager.Players;
        for (int i = 0; i < players.Count; i++) {
            players[i].ShuffleDeck();
            for (int j = 0; j < initialHandSize; j++)
                players[i].DrawCard();
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SwitchStateClientRpc() {
        stateManager.SwitchState(stateManager.UntapPhase);
    }
}