using System;
using Unity.Netcode;
using UnityEngine;

public class EndPhase : NetworkBehaviour, DuelState {
    public EventHandler<PlayerEventArgs> OnEndPhase;

    [SerializeField] private DuelStateManager stateManager;

    public void EnterState() {
        Debug.Log("End of Turn");
        OnEndPhase?.Invoke(this, new PlayerEventArgs(stateManager.DuelManager.GetCurrentPlayerTurn()));
        stateManager.DuelManager.NextTurn();
        stateManager.SwitchState(stateManager.UntapPhase);
    }

    public void UpdateState() { }
}