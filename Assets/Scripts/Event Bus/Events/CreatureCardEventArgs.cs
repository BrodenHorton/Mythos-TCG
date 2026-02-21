using System;

public class CreatureCardEventArgs : EventArgs {
    private CreatureCard card;

    public CreatureCardEventArgs(CreatureCard card) {
        this.card = card;
    }

    public CreatureCard Card { get { return card; } }
}