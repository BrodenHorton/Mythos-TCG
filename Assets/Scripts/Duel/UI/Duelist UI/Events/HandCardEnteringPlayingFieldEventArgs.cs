using System;

public class HandCardEnteringPlayingFieldEventArgs : EventArgs {
    private PlayerPlayingFieldUI playingFieldUI;
    private HandCardUI cardUI;
    private int cardIndex;

    public HandCardEnteringPlayingFieldEventArgs(PlayerPlayingFieldUI playingFieldUI, HandCardUI cardUI, int cardIndex) {
        this.playingFieldUI = playingFieldUI;
        this.cardUI = cardUI;
        this.cardIndex = cardIndex;
    }

    public PlayerPlayingFieldUI PlayingFieldUI { get { return playingFieldUI; } }

    public HandCardUI CardUI { get { return cardUI; } }

    public int CardIndex { get { return cardIndex; } }

}