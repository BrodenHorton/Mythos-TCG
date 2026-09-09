using System;
using Unity.Netcode;
using UnityEngine;

public class StartPhase : NetworkBehaviour, DuelState {
    public event EventHandler<PlayerEventArgs> OnStartPhaseEntered;
    public event EventHandler<PlayerEventArgs> OnStartPhaseEnteredFinished;

    private DuelStateManager stateManager;

    private void Start() {
        stateManager = ServiceLocator.Get<DuelStateManager>();
    }

    public void EnterState() {
        if (!IsServer)
            return;

        MatchPlayer player = stateManager.DuelManager.GetCurrentPlayerTurn();
        InvokeOnUntapPhaseEnteredClientRpc(player.PlayerId);
        OnStartPhaseEnteredFinished?.Invoke(this, new PlayerEventArgs(player.PlayerId));
        for (int i = 0; i < player.Creatures.Count; i++) {
            if (player.Creatures[i].IsTapped)
                player.Creatures[i].Untap();
        }
        stateManager.SwitchState(stateManager.DrawPhase);
    }

    public void UpdateState() { }

    [Rpc(SendTo.ClientsAndHost)]
    private void InvokeOnUntapPhaseEnteredClientRpc(ulong playerId) {
        Debug.Log("Entered Untap Phase");
        OnStartPhaseEntered?.Invoke(this, new PlayerEventArgs(playerId));
    }

    public bool CanPlaySetupCards() {
        return false;
    }

    public bool CanPlaySpellCards() {
        return false;
    }
}
