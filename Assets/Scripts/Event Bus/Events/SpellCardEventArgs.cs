using System;

public class SpellCardEventArgs : EventArgs {
    private SpellCard card;

    public SpellCardEventArgs(SpellCard card) {
        this.card = card;
    }

    public SpellCard Card { get { return card; } }
}