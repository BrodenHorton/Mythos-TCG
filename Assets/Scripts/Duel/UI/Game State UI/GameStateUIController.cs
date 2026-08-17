using System;
using UnityEngine;

public class GameStateUIController : MonoBehaviour {
    [SerializeField] private GameStateUI gameStateUI;

    private void Start() {
        DuelManager duelManager = FindFirstObjectByType<DuelManager>();
        if (duelManager == null)
            throw new Exception("Could not find DuelManager object");
        DuelStateManager stateManager = FindFirstObjectByType<DuelStateManager>();
        if (stateManager == null)
            throw new Exception("Could not find DuelStateManager object");

        stateManager.StartPhase.OnStartPhaseEntered += OnStartPhase;
        stateManager.FirstMainPhase.OnFirstMainPhaseEntered += OnFirstMainPhase;
        stateManager.CombatPhase.OnCombatPhaseEntered += OnCombatPhase;
        stateManager.SecondMainPhase.OnSecondMainPhaseEntered += OnSecondMainPhase;
        stateManager.EndPhase.OnEndPhasEntered += OnEndPhase;
        duelManager.OnNextPlayerTurnClient += SetPlayerTurnIndex;
        duelManager.OnNextFullTurn += SetFullTurn;
    }

    public void OnStartPhase(object sender, ulong playerId) {
        gameStateUI.SetDuelPhase("Start Phase");
    }

    public void OnFirstMainPhase(object sender, ulong playerId) {
        gameStateUI.SetDuelPhase("First Main Phase");
    }

    public void OnCombatPhase(object sender, ulong playerId) {
        gameStateUI.SetDuelPhase("Combat Phase");
    }

    public void OnSecondMainPhase(object sender, ulong playerId) {
        gameStateUI.SetDuelPhase("Second Main Phase");
    }

    public void OnEndPhase(object sender, ulong playerId) {
        gameStateUI.SetDuelPhase("End Phase");
    }

    public void SetPlayerTurnIndex(object sender, int playerIndex) {
        gameStateUI.SetPlayerTurnIndex(playerIndex);
    }

    public void SetFullTurn(object sender, int fullTurnCount) {
        gameStateUI.SetFullTurn(fullTurnCount);
    }
}