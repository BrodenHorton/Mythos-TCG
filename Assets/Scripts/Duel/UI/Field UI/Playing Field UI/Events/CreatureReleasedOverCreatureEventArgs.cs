using System;

public class CreatureReleasedOverCreatureEventArgs : EventArgs {
    private ulong draggingPlayerId;
    private CreatureCard heldCard;
    private CreatureCard hoveredCard;

    public CreatureReleasedOverCreatureEventArgs(ulong draggingPlayerId, CreatureCard heldCard, CreatureCard hoveredCard) {
        this.draggingPlayerId = draggingPlayerId;
        this.heldCard = heldCard;
        this.hoveredCard = hoveredCard;
    }
    
    public ulong DraggingPlayerId {  get { return draggingPlayerId; } }

    public CreatureCard HeldCard { get { return heldCard; } }

    public CreatureCard HoveredCard { get { return hoveredCard; } }
}