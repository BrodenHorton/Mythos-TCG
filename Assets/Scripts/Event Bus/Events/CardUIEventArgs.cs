using System;

public class CardUIEventArgs<T> : EventArgs where T : CardUI {
    private T cardUI;
    private bool isCanceled;

    public CardUIEventArgs(T cardUI) {
        this.cardUI = cardUI;
        isCanceled = false;
    }

    public T CardUI { get { return cardUI; } }

    public bool IsCanceled { get { return isCanceled; } set { isCanceled = value; } }
}