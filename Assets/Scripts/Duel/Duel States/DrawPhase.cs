using System;
using Unity.Netcode;
using UnityEngine;

public class DrawPhase : NetworkBehaviour, DuelState {
    public event EventHandler<PlayerEventArgs> OnDrawPhase;

    private DuelStateManager stateManager;

    private void Start() {
        stateManager = FindFirstObjectByType<DuelStateManager>();
        if (stateManager == null)
            throw new Exception("Could not find DuelStateManager object");
    }

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

    public bool CanPlaySpellCards() {
        return false;
    }
}
