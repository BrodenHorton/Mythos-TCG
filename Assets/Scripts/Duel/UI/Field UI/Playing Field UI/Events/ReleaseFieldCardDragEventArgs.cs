using System;

public class ReleaseFieldCardDragEventArgs : EventArgs {
    private PlayingFieldUI playingFieldUI;
    private FieldCardUI cardUI;
    private bool isReleasedInCombatArea;

    public ReleaseFieldCardDragEventArgs(PlayingFieldUI playingFieldUI, FieldCardUI cardUI, bool isReleasedInCombatArea) {
        this.playingFieldUI = playingFieldUI;
        this.cardUI = cardUI;
        this.isReleasedInCombatArea = isReleasedInCombatArea;
    }

    public PlayingFieldUI PlayingFieldUI { get { return playingFieldUI; } }

    public FieldCardUI CardUI { get { return cardUI; } }

    public bool IsReleasedInCombatArea { get { return isReleasedInCombatArea; } }
}