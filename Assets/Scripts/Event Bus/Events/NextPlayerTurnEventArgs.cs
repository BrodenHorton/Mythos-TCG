public class NextPlayerTurnEventArgs {
    private MatchPlayer player;
    private int playerIndex;

    public NextPlayerTurnEventArgs(MatchPlayer player, int playerIndex) {
        this.player = player;
        this.playerIndex = playerIndex;
    }

    public MatchPlayer Player { get { return player; } }

    public int PlayerIndex { get { return playerIndex; } }
}
