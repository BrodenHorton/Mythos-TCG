using System;

public class CreatureFieldCardEnteringCombatAreaEventArgs : EventArgs {
    private CombatFieldUI combatFieldUI;
    private ulong targetPlayerId;
    private CreatureFieldCardUI cardUI;

    public CreatureFieldCardEnteringCombatAreaEventArgs(CombatFieldUI combatFieldUI, CreatureFieldCardUI cardUI) {
        this.combatFieldUI = combatFieldUI;
        targetPlayerId = combatFieldUI.TargetPlayerId;
        this.cardUI = cardUI;
    }

    public CombatFieldUI CombatFieldUI { get { return combatFieldUI; } }

    public ulong TargetPlayerId {  get { return targetPlayerId; } }

    public CreatureFieldCardUI CardUI { get { return cardUI; } }
}
