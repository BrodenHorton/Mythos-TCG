using System;

public class SelectHandCardDragEventArgs : EventArgs {
    private PlayerUI playerUI;
    private HandCardUI cardUI;
    private int cardIndex;
    bool isCancelled;

    public SelectHandCardDragEventArgs(PlayerUI playerUI, HandCardUI cardUI, int cardIndex) {
        this.playerUI = playerUI;
        this.cardUI = cardUI;
        this.cardIndex = cardIndex;
    }

    public PlayerUI PlayerUI { get { return playerUI; } }

    public HandCardUI CardUI { get { return cardUI; } }

    public int CardIndex { get { return cardIndex; } }

    public bool IsCancelled { get { return isCancelled; } set { isCancelled = value; } }
}
