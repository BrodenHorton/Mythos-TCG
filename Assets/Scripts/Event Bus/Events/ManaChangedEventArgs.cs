using System;

public class ManaChangedEventArgs : EventArgs {
    private MatchPlayer player;
    private int currentMana;

    public ManaChangedEventArgs(MatchPlayer player, int currentMana) {
        this.player = player;
        this.currentMana = currentMana;
    }

    public MatchPlayer Player { get { return player; } }

    public int CurrentMana { get { return currentMana; } }
}