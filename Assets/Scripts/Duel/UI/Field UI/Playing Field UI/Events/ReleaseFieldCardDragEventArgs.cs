using System;

public class ReleaseFieldCardDragEventArgs : EventArgs {
    private PlayingFieldUI playingFieldUI;
    private FieldCardUI cardUI;

    public ReleaseFieldCardDragEventArgs(PlayingFieldUI playingFieldUI, FieldCardUI cardUI) {
        this.playingFieldUI = playingFieldUI;
        this.cardUI = cardUI;
    }

    public PlayingFieldUI PlayingFieldUI { get { return playingFieldUI; } }

    public FieldCardUI CardUI { get { return cardUI; } }
}
