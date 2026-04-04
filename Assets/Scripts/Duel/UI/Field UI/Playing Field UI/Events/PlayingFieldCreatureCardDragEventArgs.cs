using System;

public class PlayingFieldCreatureCardDragEventArgs : EventArgs {
    private PlayingFieldUI playingFieldUI;
    private CreatureFieldCardUI cardUI;
    private bool isCancelled;

    public PlayingFieldCreatureCardDragEventArgs(PlayingFieldUI playingFieldUI, CreatureFieldCardUI cardUI) {
        this.playingFieldUI = playingFieldUI;
        this.cardUI = cardUI;
    }

    public PlayingFieldUI PlayingFieldUI { get { return playingFieldUI; } }

    public CreatureFieldCardUI CardUI { get { return cardUI; } }

    public bool IsCancelled { get { return isCancelled; } set { isCancelled = value; } }
}
