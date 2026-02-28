using System;
using UnityEngine;

public class GameStateUIController : MonoBehaviour {
    [SerializeField] private GameStateUI gameStateUI;

    private void Start() {
        DuelStateManager stateManager = FindFirstObjectByType<DuelStateManager>();
        if (stateManager == null)
            throw new Exception("Could not find DuelStateManager object");

        stateManager.UntapPhase.OnUntapPhase += OnUntapPhase;
        stateManager.FirstMainPhase.OnFirstMainPhase += OnFirstMainPhase;
        stateManager.CombatPhase.OnCombatPhase += OnCombatPhase;
        stateManager.SecondMainPhase.OnSecondMainPhase += OnSecondMainPhase;
        stateManager.EndPhase.OnEndPhase += OnEndPhase;

        stateManager.UntapPhase.OnUntapPhase += SetPlayerTurnIndex;
    }

    public void OnUntapPhase(object sender, PlayerEventArgs args) {
        gameStateUI.SetDuelPhase("Untap Phase");
    }

    public void OnFirstMainPhase(object sender, PlayerEventArgs args) {
        gameStateUI.SetDuelPhase("First Main Phase");
    }

    public void OnCombatPhase(object sender, PlayerEventArgs args) {
        gameStateUI.SetDuelPhase("Combat Phase");
    }

    public void OnSecondMainPhase(object sender, PlayerEventArgs args) {
        gameStateUI.SetDuelPhase("Second Main Phase");
    }

    public void OnEndPhase(object sender, PlayerEventArgs args) {
        gameStateUI.SetDuelPhase("End Phase");
    }

    public void SetPlayerTurnIndex(object sender, PlayerEventArgs args) {

    }

    public void SetFullTurn(object sender, PlayerEventArgs args) {

    }
}