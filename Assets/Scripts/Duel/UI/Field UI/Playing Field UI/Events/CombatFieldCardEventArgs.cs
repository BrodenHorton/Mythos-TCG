using System;

public class CombatFieldCardEventArgs : EventArgs {
    private CombatFieldUI combatFieldUI;
    private CreatureFieldCardUI cardUI;

    public CombatFieldCardEventArgs(CombatFieldUI combatFieldUI, CreatureFieldCardUI cardUI) {
        this.combatFieldUI = combatFieldUI;
        this.cardUI = cardUI;
    }

    public CombatFieldUI CombatFieldUI { get { return combatFieldUI; } }

    public CreatureFieldCardUI CardUI { get { return cardUI; } }
}