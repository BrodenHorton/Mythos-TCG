using System;

public class SelectAttackerToDefendEventArgs : EventArgs {
    private CombatFieldUI combatFieldUI;
    private ulong targetPlayerId;
    private CreatureFieldCardUI attacker;
    private CreatureFieldCardUI defender;

    public SelectAttackerToDefendEventArgs(CombatFieldUI combatFieldUI, CreatureFieldCardUI attacker, CreatureFieldCardUI defender) {
        this.combatFieldUI = combatFieldUI;
        targetPlayerId = combatFieldUI.TargetPlayerId;
        this.attacker = attacker;
        this.defender = defender;
    }

    public CombatFieldUI CombatFieldUI { get { return combatFieldUI; } }

    public ulong TargetPlayerId { get { return targetPlayerId; } }

    public CreatureFieldCardUI Attacker { get { return attacker; } }

    public CreatureFieldCardUI Defender { get { return defender; } }
}