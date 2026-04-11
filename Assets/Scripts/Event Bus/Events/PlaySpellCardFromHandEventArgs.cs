using System;

public class PlaySpellCardFromHandEventArgs : EventArgs {
    private MatchPlayer player;
    private SpellCard card;
    private int handIndex;

    public PlaySpellCardFromHandEventArgs(MatchPlayer player, SpellCard card, int handIndex) {
        this.player = player;
        this.card = card;
        this.handIndex = handIndex;
    }

    public MatchPlayer Player { get { return player; } }

    public SpellCard Card { get { return card; } }

    public int HandIndex { get { return handIndex; } }
}