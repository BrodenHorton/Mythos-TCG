using System;

public class CreatureFieldCardDragEventArgs : EventArgs {
    private PlayerPlayingFieldUI playingFieldUI;
    private CreatureFieldCardUI cardUI;
    private bool isCancelled;

    public CreatureFieldCardDragEventArgs(PlayerPlayingFieldUI playingFieldUI, CreatureFieldCardUI cardUI) {
        this.playingFieldUI = playingFieldUI;
        this.cardUI = cardUI;
        isCancelled = false;
    }

    public PlayerPlayingFieldUI PlayingFieldUI { get { return playingFieldUI; } }

    public CreatureFieldCardUI CardUI { get { return cardUI; } }

    public bool IsCancelled { get { return isCancelled; } set { isCancelled = value; } }
}
