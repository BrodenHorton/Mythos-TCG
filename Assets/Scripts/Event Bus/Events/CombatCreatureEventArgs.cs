using System;

public class CombatCreatureEventArgs : EventArgs {
    private ulong initiatorId;
    private ulong targetId;
    private CreatureCard card;

    public CombatCreatureEventArgs(ulong initiatorId, ulong targetId, CreatureCard card) {
        this.initiatorId = initiatorId;
        this.targetId = targetId;
        this.card = card;
    }

    public ulong InitiatorId { get { return initiatorId; } }

    public ulong TargetId { get { return targetId; } }

    public CreatureCard Card { get { return card; } }
}
