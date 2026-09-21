using System;

public class CreatureReleasedOverCreatureEventArgs : PlayerEventArgs {
    private CreatureCard heldCard;
    private CreatureCard hoveredCard;

    public CreatureReleasedOverCreatureEventArgs(ulong draggingPlayerId, CreatureCard heldCard, CreatureCard hoveredCard) : base(draggingPlayerId) {
        this.heldCard = heldCard;
        this.hoveredCard = hoveredCard;
    }

    public CreatureCard HeldCard { get { return heldCard; } }

    public CreatureCard HoveredCard { get { return hoveredCard; } }
}