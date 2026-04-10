using System;

public class ReleaseCreatureFieldCardDragEventArgs : EventArgs {
    private PlayerPlayingFieldUI playingFieldUI;
    private CreatureFieldCardUI cardUI;

    public ReleaseCreatureFieldCardDragEventArgs(PlayerPlayingFieldUI playingFieldUI, CreatureFieldCardUI cardUI) {
        this.playingFieldUI = playingFieldUI;
        this.cardUI = cardUI;
    }

    public PlayerPlayingFieldUI PlayingFieldUI { get { return playingFieldUI; } }

    public CreatureFieldCardUI CardUI { get { return cardUI; } }
}
