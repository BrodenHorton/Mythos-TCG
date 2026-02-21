public class PlayerCardEventArgs {
    private MatchPlayer player;
    private Card card;

    public PlayerCardEventArgs(MatchPlayer player, Card card) {
        this.player = player;
        this.card = card;
    }

    public MatchPlayer Player { get { return player; } }

    public Card Card { get { return card; } }
}
