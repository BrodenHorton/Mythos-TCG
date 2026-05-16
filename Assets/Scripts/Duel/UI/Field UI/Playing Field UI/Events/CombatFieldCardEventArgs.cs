using System;

public class CombatFieldCardEventArgs<T> : EventArgs where T : FieldCardUI {
    private CombatFieldUI combatFieldUI;
    private T cardUI;

    public CombatFieldCardEventArgs(CombatFieldUI combatFieldUI, T cardUI) {
        this.combatFieldUI = combatFieldUI;
        this.cardUI = cardUI;
    }

    public CombatFieldUI CombatFieldUI { get { return combatFieldUI; } }

    public T CardUI { get { return cardUI; } }
}