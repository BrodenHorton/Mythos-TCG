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

        stateManager.UntapPhase.OnUntapPhase += OnUntapPhase;
        stateManager.FirstMainPhase.OnFirstMainPhase += OnFirstMainPhase;
        stateManager.CombatPhase.OnCombatPhase += OnCombatPhase;
        stateManager.SecondMainPhase.OnSecondMainPhase += OnSecondMainPhase;
        stateManager.EndPhase.OnEndPhase += OnEndPhase;
        duelManager.OnNextPlayerTurnClient += SetPlayerTurnIndex;
        duelManager.OnNextFullTurn += SetFullTurn;
    }

    public void OnUntapPhase(object sender, ulong playerId) {
        gameStateUI.SetDuelPhase("Untap Phase");
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