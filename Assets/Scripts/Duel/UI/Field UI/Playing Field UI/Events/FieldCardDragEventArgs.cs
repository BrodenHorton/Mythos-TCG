using System;

public class FieldCardDragEventArgs<T> : EventArgs where T : FieldCardUI {
    private PlayerPlayingFieldUI playingFieldUI;
    private T cardUI;
    private bool isCancelled;

    public FieldCardDragEventArgs(PlayerPlayingFieldUI playingFieldUI, T cardUI) {
        this.playingFieldUI = playingFieldUI;
        this.cardUI = cardUI;
        isCancelled = false;
    }

    public PlayerPlayingFieldUI PlayingFieldUI { get { return playingFieldUI; } }

    public T CardUI { get { return cardUI; } }

    public bool IsCancelled { get { return isCancelled; } set { isCancelled = value; } }
}
