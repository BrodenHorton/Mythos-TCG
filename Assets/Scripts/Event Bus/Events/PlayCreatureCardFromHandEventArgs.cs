using System;

public class PlayCreatureCardFromHandEventArgs : EventArgs {
    private MatchPlayer player;
    private CreatureCard card;
    private int handIndex;

    public PlayCreatureCardFromHandEventArgs(MatchPlayer player, CreatureCard card, int handIndex) {
        this.player = player;
        this.card = card;
        this.handIndex = handIndex;
    }

    public MatchPlayer Player { get { return player; } }

    public CreatureCard Card { get { return card; } }

    public int HandIndex { get { return handIndex; } }
}