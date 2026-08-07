using System;

public class CardUIEventArgs<T> : EventArgs where T : CardUI {
    private CardUI cardUI;
    private bool isCanceled;

    public CardUIEventArgs(CardUI cardUI) {
        this.cardUI = cardUI;
        isCanceled = false;
    }

    public CardUI CardUI { get { return cardUI; } }

    public bool IsCanceled { get { return isCanceled; } set { isCanceled = value; } }
}