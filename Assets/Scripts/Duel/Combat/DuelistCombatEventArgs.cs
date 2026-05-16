using System;
using System.Collections.Generic;

public class DuelistCombatEventArgs : EventArgs {
    private ulong initiatorId;
    private ulong targetId;
    private List<CreatureCombat> creatureCombates;

    public DuelistCombatEventArgs(ulong initiatorId, ulong targetId, List<CreatureCombat> creatureCombates) {
        this.initiatorId = initiatorId;
        this.targetId = targetId;
        this.creatureCombates = creatureCombates;
    }

    public ulong InitiatorId { get { return initiatorId; } }

    public ulong TargetId { get { return targetId; } }

    public List<CreatureCombat> CreatureCombates { get { return creatureCombates; } }
}