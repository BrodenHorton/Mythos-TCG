using System;

public class ReleaseCreatureFieldCardDragEventArgs : EventArgs {
    private PlayingFieldUI playingFieldUI;
    private CreatureFieldCardUI cardUI;

    public ReleaseCreatureFieldCardDragEventArgs(PlayingFieldUI playingFieldUI, CreatureFieldCardUI cardUI) {
        this.playingFieldUI = playingFieldUI;
        this.cardUI = cardUI;
    }

    public PlayingFieldUI PlayingFieldUI { get { return playingFieldUI; } }

    public CreatureFieldCardUI CardUI { get { return cardUI; } }
}
