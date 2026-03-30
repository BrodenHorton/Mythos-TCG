using System;

public class SelectFieldCardDragEventArgs : EventArgs {
    private PlayingFieldUI playingFieldUI;
    private FieldCardUI cardUI;
    bool isCancelled;

    public SelectFieldCardDragEventArgs(PlayingFieldUI playingFieldUI, FieldCardUI cardUI) {
        this.playingFieldUI = playingFieldUI;
        this.cardUI = cardUI;
    }

    public PlayingFieldUI PlayingFieldUI { get { return playingFieldUI; } }

    public FieldCardUI CardUI { get { return cardUI; } }

    public bool IsCancelled { get { return isCancelled; } set { isCancelled = value; } }
}
