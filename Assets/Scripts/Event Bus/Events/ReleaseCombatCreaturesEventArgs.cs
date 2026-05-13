using System;
using System.Collections.Generic;

public class ReleaseCombatCreaturesEventArgs : EventArgs {
    private ulong playerId;
    private List<CreatureFieldCardUI> creatures;

    public ReleaseCombatCreaturesEventArgs(ulong playerId, List<CreatureFieldCardUI> creatures) {
        this.playerId = playerId;
        this.creatures = creatures;
    }

    public ulong PlayerId { get { return playerId; } }

    public List<CreatureFieldCardUI> Creatures { get { return creatures; } }
}