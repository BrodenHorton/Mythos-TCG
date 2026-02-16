using System;

public class PlaySpellCardEventArgs : EventArgs {
    private MatchPlayer player;
    private SpellCard card;

    public PlaySpellCardEventArgs(MatchPlayer player, SpellCard card) {
        this.player = player;
        this.card = card;
    }

    public MatchPlayer Player { get { return player; } }

    public SpellCard Card { get { return card; } }
}
