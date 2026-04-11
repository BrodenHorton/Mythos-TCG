using System;

public class CombatFieldCardSelectEventArgs : EventArgs {
    private CombatFieldUI combatFieldUI;
    private FieldCardUI cardUI;

    public CombatFieldCardSelectEventArgs(CombatFieldUI combatFieldUI, FieldCardUI cardUI) {
        this.combatFieldUI = combatFieldUI;
        this.cardUI = cardUI;
    }

    public CombatFieldUI CombatFieldUI { get { return combatFieldUI; } }

    public FieldCardUI CardUI { get { return cardUI; } }
}