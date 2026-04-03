using System;

public class ReleaseFieldCardDragPlayingFieldEventArgs : EventArgs {
    private PlayingFieldUI playingFieldUI;
    private CreatureFieldCardUI cardUI;

    public ReleaseFieldCardDragPlayingFieldEventArgs(PlayingFieldUI playingFieldUI, CreatureFieldCardUI cardUI) {
        this.playingFieldUI = playingFieldUI;
        this.cardUI = cardUI;
    }

    public PlayingFieldUI PlayingFieldUI { get { return playingFieldUI; } }

    public CreatureFieldCardUI CardUI { get { return cardUI; } }
}
