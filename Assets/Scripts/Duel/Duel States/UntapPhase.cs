using System;
using Unity.Netcode;
using UnityEngine;

public class UntapPhase : NetworkBehaviour, DuelState {
    public event EventHandler<PlayerEventArgs> OnUntapPhase;

    [SerializeField] private DuelStateManager stateManager;

    public void EnterState() {
        if (!IsServer)
            return;

        UntapPhaseClientRpc();
    }

    [ClientRpc]
    private void UntapPhaseClientRpc() {
        Debug.Log("Entered Untap Phase");
        OnUntapPhase?.Invoke(this, new PlayerEventArgs(stateManager.DuelManager.GetCurrentPlayerTurn()));
        MatchPlayer player = stateManager.DuelManager.GetCurrentPlayerTurn();
        for (int i = 0; i < player.Creatures.Count; i++) {
            if (player.Creatures[i].IsTapped)
                player.Creatures[i].Untap();
        }
        stateManager.SwitchState(stateManager.DrawPhase);
    }

    public void UpdateState() {

    }
}
