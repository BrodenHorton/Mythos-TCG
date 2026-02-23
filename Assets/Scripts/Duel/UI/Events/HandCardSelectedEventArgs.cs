using System;

public class HandCardSelectedEventArgs : EventArgs {
    private HandCardUI cardUI;

    public HandCardSelectedEventArgs(HandCardUI cardUI) {
               this.cardUI = cardUI;
    }

    public HandCardUI CardUI { get { return cardUI; } }
}
