using System;
using Unity.Netcode;
using UnityEngine;

public class InitializationPhase : NetworkBehaviour, DuelState {
    public event EventHandler OnInitializationPhase;

    [SerializeField] private DuelStateManager stateManager;

    private int initialHandSize = 5;

    public void EnterState() {
        if (!IsServer)
            return;

        InitializationPhaseClientRpc();
    }

    [ClientRpc]
    private void InitializationPhaseClientRpc() {
        Debug.Log("Entered Initialization Phase");
        OnInitializationPhase?.Invoke(this, EventArgs.Empty);
        MatchPlayer player = stateManager.DuelManager.LocalClientPlayer;
        player.ShuffleDeck();
        for (int i = 0; i < initialHandSize; i++)
            player.DrawCard();
        stateManager.SwitchState(stateManager.UntapPhase);
    }

    public void UpdateState() {

    }
}