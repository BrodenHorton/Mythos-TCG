using System;

public class ReleaseFieldCardDragEventArgs<T> : EventArgs where T : FieldCardUI {
    private PlayerPlayingFieldUI playingFieldUI;
    private T cardUI;

    public ReleaseFieldCardDragEventArgs(PlayerPlayingFieldUI playingFieldUI, T cardUI) {
        this.playingFieldUI = playingFieldUI;
        this.cardUI = cardUI;
    }

    public PlayerPlayingFieldUI PlayingFieldUI { get { return playingFieldUI; } }

    public T CardUI { get { return cardUI; } }
}
