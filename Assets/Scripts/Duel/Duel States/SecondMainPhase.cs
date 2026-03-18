using System;
using Unity.Netcode;
using UnityEngine;

public class SecondMainPhase : NetworkBehaviour, DuelState {
    public event EventHandler<PlayerEventArgs> OnSecondMainPhase;

    [SerializeField] private DuelStateManager stateManager;

    public void EnterState() {
        Debug.Log("Entered Second Main Phase");
        OnSecondMainPhase?.Invoke(this, new PlayerEventArgs(stateManager.DuelManager.GetCurrentPlayerTurn()));
        if (stateManager.DuelManager.IsLocalClientPlayerTurn())
            EventBus.OnActionButtonPressed += NextPhase;
        else
            stateManager.SwitchState(stateManager.EndPhase);
    }

    public void UpdateState() {

    }

    private void NextPhase(object sender, EventArgs args) {
        EventBus.OnActionButtonPressed -= NextPhase;
        stateManager.SwitchState(stateManager.EndPhase);
    }
}