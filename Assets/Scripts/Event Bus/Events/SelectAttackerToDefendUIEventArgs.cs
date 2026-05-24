using System;

public class SelectAttackerToDefendUIEventArgs : EventArgs {
    private CombatFieldUI combatFieldUI;
    private CreatureFieldCardUI attacker;
    private CreatureFieldCardUI defender;

    public SelectAttackerToDefendUIEventArgs(CombatFieldUI combatFieldUI, CreatureFieldCardUI attacker, CreatureFieldCardUI defender) {
        this.combatFieldUI = combatFieldUI;
        this.attacker = attacker;
        this.defender = defender;
    }

    public CombatFieldUI CombatFieldUI { get { return combatFieldUI; } }

    public CreatureFieldCardUI Attacker { get { return attacker; } }

    public CreatureFieldCardUI Defender { get { return defender; } }
}