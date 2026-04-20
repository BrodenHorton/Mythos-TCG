public interface DuelAction {
    void AddListeners();

    void RemoveListeners();

    void Execute();

    string ActiveActionMessage();

    string InactiveActionMessage();
}