using System;

public class DuelistCombatEventArgs : EventArgs {
    private DuelistCombat combat;

    public DuelistCombatEventArgs(DuelistCombat combat) {
        this.combat = combat;
    }

    public DuelistCombat Combat { get { return combat; } }
}