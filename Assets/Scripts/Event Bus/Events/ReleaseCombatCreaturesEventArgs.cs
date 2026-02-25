using System;
using System.Collections.Generic;

public class ReleaseCombatCreaturesEventArgs : EventArgs {
    private MatchPlayer player;
    private List<CreatureFieldCardUI> creatures;

    public ReleaseCombatCreaturesEventArgs(MatchPlayer player, List<CreatureFieldCardUI> creatures) {
        this.player = player;
        this.creatures = creatures;
    }

    public MatchPlayer Player { get { return player; } }

    public List<CreatureFieldCardUI> Creatures { get { return creatures; } }
}