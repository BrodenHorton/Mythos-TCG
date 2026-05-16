using System;

public class PlayingFieldCardEventArgs<T> : EventArgs where T : FieldCardUI {
    private PlayerPlayingFieldUI playingFieldUI;
    private T cardUI;
    private bool isCanceled;

    public PlayingFieldCardEventArgs(PlayerPlayingFieldUI playingFieldUI, T cardUI) {
        this.playingFieldUI = playingFieldUI;
        this.cardUI = cardUI;
        isCanceled = false;
    }

    public PlayerPlayingFieldUI PlayingFieldUI { get { return playingFieldUI; } }

    public T CardUI { get { return cardUI; } }

    public bool IsCanceled { get { return isCanceled; } }
}
