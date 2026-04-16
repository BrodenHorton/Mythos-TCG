public interface CombatState : PlayableState {
    void EnterState();

    void UpdateState();

    bool CanDeclareAttackers();

    bool CanDeclareDefenders();
}
