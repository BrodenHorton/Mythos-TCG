
public class NextFullTurnEventArgs {
    private int fullTurnCount;

    public NextFullTurnEventArgs(int fullTurnCount) {
        this.fullTurnCount = fullTurnCount;
    }

    public int FullTurnCount { get { return fullTurnCount; } }
}