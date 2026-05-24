using System;

public class CombatCreaturePayloadEventArgs : EventArgs {
    private ulong initiatorId;
    private ulong targetId;
    private CreatureCardPayload card;

    public CombatCreaturePayloadEventArgs(ulong initiatorId, ulong targetId, CreatureCardPayload card) {
        this.initiatorId = initiatorId;
        this.targetId = targetId;
        this.card = card;
    }

    public ulong InitiatorId { get { return initiatorId; } }

    public ulong TargetId { get { return targetId; } }

    public CreatureCardPayload Card { get { return card; } }
}