using System;

public class ReleaseFieldCardDragOverCombatFieldEventArgs : EventArgs {
    private PlayingFieldUI playingFieldUI;
    private FieldCardUI cardUI;
    private ulong targetPlayerId;

    public ReleaseFieldCardDragOverCombatFieldEventArgs(PlayingFieldUI playingFieldUI, FieldCardUI cardUI, ulong targetPlayerId) {
        this.playingFieldUI = playingFieldUI;
        this.cardUI = cardUI;
        this.targetPlayerId = targetPlayerId;
    }

    public PlayingFieldUI PlayingFieldUI { get { return playingFieldUI; } }

    public FieldCardUI CardUI { get { return cardUI; } }

    public ulong TargetPlayerId { get { return targetPlayerId; } }
}