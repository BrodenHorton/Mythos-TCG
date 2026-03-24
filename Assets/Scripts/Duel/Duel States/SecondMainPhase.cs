using System;
using Unity.Netcode;
using UnityEngine;

public class SecondMainPhase : NetworkBehaviour, DuelState {
    public event EventHandler<PlayerEventArgs> OnSecondMainPhase;

    [SerializeField] private DuelStateManager stateManager;

    public void EnterState() {
        Debug.Log("Entered Second Main Phase");
        OnSecondMainPhase?.Invoke(this, new PlayerEventArgs(stateManager.DuelManager.GetCurrentPlayerTurn()));
        if (IsServer) {
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
        EventBus.OnActionButtonPressed += NextPhase;
    }

    private void NextPhase(object sender, EventArgs args) {
        EventBus.OnActionButtonPressed -= NextPhase;
        SwitchStateServerRpc();
    }

    [Rpc(SendTo.Server)]
    private void SwitchStateServerRpc() {
        SwitchStateClientRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SwitchStateClientRpc() {
        stateManager.SwitchState(stateManager.EndPhase);
    }
}