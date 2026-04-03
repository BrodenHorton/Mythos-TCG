using System;

public class PlayingFieldCardDragEventArgs : EventArgs {
    private PlayingFieldUI playingFieldUI;
    private FieldCardUI cardUI;
    private bool isCancelled;

    public PlayingFieldCardDragEventArgs(PlayingFieldUI playingFieldUI, FieldCardUI cardUI) {
        this.playingFieldUI = playingFieldUI;
        this.cardUI = cardUI;
    }

    public PlayingFieldUI PlayingFieldUI { get { return playingFieldUI; } }

    public FieldCardUI CardUI { get { return cardUI; } }

    public bool IsCancelled { get { return isCancelled; } set { isCancelled = value; } }
}
