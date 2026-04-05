using System;

public class HandCardDragEventArgs : EventArgs {
    private PlayerUI playerUI;
    private HandCardUI cardUI;
    private int cardIndex;
    private bool isCancelled;

    public HandCardDragEventArgs(PlayerUI playerUI, HandCardUI cardUI, int cardIndex) {
        this.playerUI = playerUI;
        this.cardUI = cardUI;
        this.cardIndex = cardIndex;
        isCancelled = false;
    }

    public PlayerUI PlayerUI { get { return playerUI; } }

    public HandCardUI CardUI { get { return cardUI; } }

    public int CardIndex { get { return cardIndex; } }

    public bool IsCancelled { get { return isCancelled; } set { isCancelled = value; } }
}