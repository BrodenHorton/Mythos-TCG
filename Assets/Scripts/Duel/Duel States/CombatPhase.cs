using System;
using Unity.Netcode;
using UnityEngine;

public class CombatPhase : NetworkBehaviour, DuelState {
    public event EventHandler<PlayerEventArgs> OnCombatPhase;
    public event EventHandler<PlayerEventArgs> OnCombatPhaseFinished;

    [SerializeField] private DuelStateManager stateManager;
    [SerializeField] private CombatManager combatManager;

    public void EnterState() {
        Debug.Log("Entered Combat Phase");
        OnCombatPhase?.Invoke(this, new PlayerEventArgs(stateManager.DuelManager.GetCurrentPlayerTurn()));
        if(IsServer) {
            ClientRpcParams clientRpcParams = new ClientRpcParams {
                Send = new ClientRpcSendParams {
                    TargetClientIds = new ulong[] { stateManager.DuelManager.GetCurrentPlayerTurn().PlayerId }
                }
            };
            EnableActionButtonForCurrentPlayersTurn(clientRpcParams);
        }
    }

    public void UpdateState() { }

    [Rpc(SendTo.ClientsAndHost)]
    private void EnableActionButtonForCurrentPlayersTurn(ClientRpcParams clientRpcParams) {
        EventBus.OnActionButtonPressed += ProcessCombat;
    }

    private void ProcessCombat(object sender, EventArgs args) {
        EventBus.OnActionButtonPressed -= ProcessCombat;
        ProcessCombatServerRpc();
    }

    [Rpc(SendTo.Server)]
    private void ProcessCombatServerRpc() {
        combatManager.ProcessCombat();
        CombatFinishedClientRpc();
        SwitchStateClientRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void CombatFinishedClientRpc() {
        OnCombatPhaseFinished?.Invoke(this, new PlayerEventArgs(stateManager.DuelManager.GetCurrentPlayerTurn()));
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SwitchStateClientRpc() {
        stateManager.SwitchState(stateManager.SecondMainPhase);
    }
}
