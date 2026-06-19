using System;

public class FieldCardEventArgs<T> : EventArgs where T : FieldCardUI {
    private T cardUI;
    private bool isCanceled;

    public FieldCardEventArgs(T cardUI) {
        this.cardUI = cardUI;
        isCanceled = false;
    }

    public T CardUI { get { return cardUI; } }

    public bool IsCanceled { get { return isCanceled; } set { isCanceled = value; } }
}