public class PlayerEventArgs {
    private MatchPlayer player;

    public PlayerEventArgs(MatchPlayer player) {
        this.player = player;
    }

    public MatchPlayer Player { get { return player; } }
}
