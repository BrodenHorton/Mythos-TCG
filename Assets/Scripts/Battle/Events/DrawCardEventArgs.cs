using System;

public class DrawCardEventArgs : EventArgs {
    private MatchPlayer player;

    public DrawCardEventArgs(MatchPlayer player) {
        this.player = player;
    }

    public MatchPlayer Player { get { return player; } }
}
