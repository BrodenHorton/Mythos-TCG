using UnityEngine;

public interface DuelState {
    void EnterState();

    void UpdateState();

    bool CanPlaySetupCards();

    bool CanPlayCombatCards();
}
