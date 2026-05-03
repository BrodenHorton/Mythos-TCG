using System.Collections.Generic;
using System;
using Unity.Netcode;
using UnityEngine;

public class InitializationPhase : NetworkBehaviour, DuelState {
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
        if (!IsServer)
            return;

        InvokeOnInitializationPhaseClientRpc();
        List<MatchPlayer> players = stateManager.DuelManager.Players;
        for(int i = 0; i < players.Count; i++) {
            players[i].ShuffleDeck();
            for (int j = 0; j < DuelManager.INITIAL_HAND_SIZE; j++)
                players[i].DrawCard();
        }
        actionManager.SetActionFocusPlayerIndices(0);
        stateManager.SwitchState(stateManager.UntapPhase);
    }

    public void UpdateState() { }

    [Rpc(SendTo.ClientsAndHost)]
    private void InvokeOnInitializationPhaseClientRpc() {
        Debug.Log("Entered Initialization Phase");
        OnInitializationPhase?.Invoke(this, EventArgs.Empty);
    }

    public bool CanPlaySetupCards() {
        return false;
    }

    public bool CanPlaySpellCards() {
        return false;
    }
}