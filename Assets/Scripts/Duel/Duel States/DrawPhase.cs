using System;
using Unity.Netcode;
using UnityEngine;

public class DrawPhase : NetworkBehaviour, DuelState {
    public event EventHandler<PlayerEventArgs> OnDrawPhase;

    [SerializeField] private DuelStateManager stateManager;

    public void EnterState() {
        Debug.Log("Entered Draw Phase");
        OnDrawPhase?.Invoke(this, new PlayerEventArgs(stateManager.DuelManager.GetCurrentPlayerTurn()));
        if(IsServer) {
            DrawPhaseServerRpc();
        }
    }

    public void UpdateState() { }

    [Rpc(SendTo.Server)]
    private void DrawPhaseServerRpc() {
        DuelManager duelManager = stateManager.DuelManager;
        duelManager.GetCurrentPlayerTurn().CurrentMana = duelManager.GetStartOfTurnManaCount();
        duelManager.GetCurrentPlayerTurn().DrawCard();
        SwitchStateClientRpc();
    }

    [Rpc(SendTo.ClientsAndHost)] 
    private void SwitchStateClientRpc() {
        stateManager.SwitchState(stateManager.FirstMainPhase);
    }
}
