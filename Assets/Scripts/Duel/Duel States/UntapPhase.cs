using System;
using Unity.Netcode;
using UnityEngine;

public class UntapPhase : NetworkBehaviour, DuelState {
    public event EventHandler<PlayerEventArgs> OnUntapPhase;

    [SerializeField] private DuelStateManager stateManager;

    public void EnterState() {
        Debug.Log("Entered Untap Phase");
        OnUntapPhase?.Invoke(this, new PlayerEventArgs(stateManager.DuelManager.GetCurrentPlayerTurn()));
        if(IsServer) {
            UntapCreaturesServerRpc();
            SwitchStateClientRpc();
        }
    }

    public void UpdateState() { }

    [Rpc(SendTo.Server)]
    private void UntapCreaturesServerRpc() {
        MatchPlayer player = stateManager.DuelManager.GetCurrentPlayerTurn();
        for (int i = 0; i < player.Creatures.Count; i++) {
            if (player.Creatures[i].IsTapped)
                player.Creatures[i].Untap();
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SwitchStateClientRpc() {
        stateManager.SwitchState(stateManager.DrawPhase);
    }
}
