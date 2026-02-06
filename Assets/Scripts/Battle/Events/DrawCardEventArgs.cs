using System;

public class DrawCardEventArgs : EventArgs {
    private MatchPlayer player;
    private int playerIndex;

    public DrawCardEventArgs(MatchPlayer player, int playerIndex) {
        this.player = player;
        this.playerIndex = playerIndex;
    }

    public MatchPlayer Player { get { return player; } }

    public int PlayerIndex { get {  return playerIndex; } }
}
