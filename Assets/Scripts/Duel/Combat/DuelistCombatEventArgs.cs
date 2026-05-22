using System;
using System.Collections.Generic;

public class DuelistCombatEventArgs : EventArgs {
    private ulong initiatorId;
    private ulong targetId;
    private List<CreatureCombatPayload> creatureCombates;

    public DuelistCombatEventArgs(ulong initiatorId, ulong targetId, List<CreatureCombatPayload> creatureCombates) {
        this.initiatorId = initiatorId;
        this.targetId = targetId;
        this.creatureCombates = creatureCombates;
    }

    public ulong InitiatorId { get { return initiatorId; } }

    public ulong TargetId { get { return targetId; } }

    public List<CreatureCombatPayload> CreatureCombates { get { return creatureCombates; } }
}