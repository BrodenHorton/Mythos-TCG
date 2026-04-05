using System;

public class HandCardEnteringPlayingFieldEventArgs : EventArgs {
    private PlayingFieldUI playingFieldUI;
    private HandCardUI cardUI;
    private int cardIndex;

    public HandCardEnteringPlayingFieldEventArgs(PlayingFieldUI playingFieldUI, HandCardUI cardUI, int cardIndex) {
        this.playingFieldUI = playingFieldUI;
        this.cardUI = cardUI;
        this.cardIndex = cardIndex;
    }

    public PlayingFieldUI PlayingFieldUI { get { return playingFieldUI; } }

    public HandCardUI CardUI { get { return cardUI; } }

    public int CardIndex { get { return cardIndex; } }

}