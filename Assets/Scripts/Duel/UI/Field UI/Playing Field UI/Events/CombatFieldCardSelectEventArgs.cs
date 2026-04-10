using System;

public class CombatFieldCardSelectEventArgs : EventArgs {
    private PlayerCombatFieldUI combatFieldUI;
    private FieldCardUI cardUI;

    public CombatFieldCardSelectEventArgs(PlayerCombatFieldUI combatFieldUI, FieldCardUI cardUI) {
        this.combatFieldUI = combatFieldUI;
        this.cardUI = cardUI;
    }

    public PlayerCombatFieldUI CombatFieldUI { get { return combatFieldUI; } }

    public FieldCardUI CardUI { get { return cardUI; } }
}