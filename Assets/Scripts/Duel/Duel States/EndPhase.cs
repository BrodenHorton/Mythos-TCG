using System;
using Unity.Netcode;
using UnityEngine;

public class EndPhase : NetworkBehaviour, DuelState {
    public EventHandler<ulong> OnEndPhase;

    private DuelStateManager stateManager;

    private void Start() {
        stateManager = ServiceLocator.Get<DuelStateManager>();
    }

    public void EnterState() {
        if (!IsServer)
            return;

        InvokeOnEndPhaseClientRpc(stateManager.DuelManager.GetCurrentPlayerTurn().PlayerId);
        stateManager.DuelManager.NextTurn();
        stateManager.SwitchState(stateManager.UntapPhase);
    }

    public void UpdateState() { }

    [Rpc(SendTo.ClientsAndHost)]
    private void InvokeOnEndPhaseClientRpc(ulong playerId) {
        Debug.Log("Entered End Phase");
        OnEndPhase?.Invoke(this, playerId);
    }

    public bool CanPlaySetupCards() {
        return false;
    }

    public bool CanPlaySpellCards() {
        return false;
    }
}