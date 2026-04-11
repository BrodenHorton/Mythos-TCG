using System;
using Unity.Netcode;
using UnityEngine;

public class DrawPhase : NetworkBehaviour, DuelState {
    public event EventHandler<PlayerEventArgs> OnDrawPhase;

    [SerializeField] private DuelStateManager stateManager;

    public void EnterState() {
        Debug.Log("Entered Draw Phase");
        OnDrawPhase?.Invoke(this, new PlayerEventArgs(stateManager.DuelManager.GetCurrentPlayerTurn()));
        DuelManager duelManager = stateManager.DuelManager;
        duelManager.GetCurrentPlayerTurn().CurrentMana = duelManager.GetStartOfTurnManaCount();
        duelManager.GetCurrentPlayerTurn().DrawCard();
        stateManager.SwitchState(stateManager.FirstMainPhase);
    }

    public void UpdateState() { }

    public bool CanPlaySetupCards() {
        return false;
    }

    public bool CanPlayCombatCards() {
        return false;
    }
}
