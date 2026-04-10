using System;

public class CreatureFieldCardEnteringCombatFieldEventArgs : EventArgs {
    private PlayerCombatFieldUI combatFieldUI;
    private ulong targetPlayerId;
    private CreatureFieldCardUI cardUI;

    public CreatureFieldCardEnteringCombatFieldEventArgs(PlayerCombatFieldUI combatFieldUI, CreatureFieldCardUI cardUI) {
        this.combatFieldUI = combatFieldUI;
        targetPlayerId = combatFieldUI.TargetPlayerId;
        this.cardUI = cardUI;
    }

    public PlayerCombatFieldUI CombatFieldUI { get { return combatFieldUI; } }

    public ulong TargetPlayerId { get { return targetPlayerId; } }

    public CreatureFieldCardUI CardUI { get { return cardUI; } }
}

