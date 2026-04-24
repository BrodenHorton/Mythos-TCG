public interface DuelistAction {

    void Execute();

    string ActiveActionMessage();

    string InactiveActionMessage();
}