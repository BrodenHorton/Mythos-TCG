using UnityEngine;

public interface DuelState : PlayableState {
    void EnterState();

    void UpdateState();
}
