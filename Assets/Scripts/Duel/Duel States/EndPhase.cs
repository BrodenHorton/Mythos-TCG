using System;
using Unity.Netcode;
using UnityEngine;

public class EndPhase : NetworkBehaviour, DuelState {
    public EventHandler<ulong> OnEndPhasEntered;
    public EventHandler<ulong> OnEndPhasEnteredFinished;

    private DuelManager duelManager;
    private DuelStateManager stateManager;

    private void Start() {
        duelManager = ServiceLocator.Get<DuelManager>();
        stateManager = ServiceLocator.Get<DuelStateManager>();
    }

    public void EnterState() {
        if (!IsServer)
            return;

        OnEndPhasEntered?.Invoke(this, duelManager.GetCurrentPlayerTurn().PlayerId);
        InvokeOnEndPhaseClientRpc(duelManager.GetCurrentPlayerTurn().PlayerId);
        duelManager.GetCurrentPlayerTurn().ClearSummoningSickness();
        duelManager.NextTurn();
        stateManager.SwitchState(stateManager.UntapPhase);
    }

    public void UpdateState() { }

    [Rpc(SendTo.ClientsAndHost)]
    private void InvokeOnEndPhaseClientRpc(ulong playerId) {
        Debug.Log("Entered End Phase");
        OnEndPhasEnteredFinished?.Invoke(this, playerId);
    }

    public bool CanPlaySetupCards() {
        return false;
    }

    public bool CanPlaySpellCards() {
        return false;
    }
}