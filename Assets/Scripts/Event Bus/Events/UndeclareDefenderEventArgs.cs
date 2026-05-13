using System;

public class UndeclareDefenderEventArgs : EventArgs {
    private ulong initiatorId;
    private ulong targetId;
    private CreatureCard defender;

    public UndeclareDefenderEventArgs(ulong initiatorId, ulong targetId, CreatureCard defender) {
        this.initiatorId = initiatorId;
        this.targetId = targetId;
        this.defender = defender;
    }

    public ulong InitiatorId { get { return initiatorId; } }

    public ulong TargetId { get { return targetId; } }

    public CreatureCard Defender { get { return defender; } }
}
