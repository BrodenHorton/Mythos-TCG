using System;

public class ReleaseHandCardDragEventArgs : EventArgs {
    private PlayerUI playerUI;
    private HandCardUI cardUI;
    private int cardIndex;
    private bool isReleasedInPlayableArea;

    public ReleaseHandCardDragEventArgs(PlayerUI playerUI, HandCardUI cardUI, int cardIndex, bool isReleasedInPlayableArea) {
        this.playerUI = playerUI;
        this.cardUI = cardUI;
        this.cardIndex = cardIndex;
        this.isReleasedInPlayableArea = isReleasedInPlayableArea;
    }

    public PlayerUI PlayerUI { get { return playerUI; } }

    public HandCardUI CardUI { get { return cardUI; } }

    public int CardIndex { get { return cardIndex; } }

    public bool IsReleasedInPlayableArea { get { return isReleasedInPlayableArea; } }
}