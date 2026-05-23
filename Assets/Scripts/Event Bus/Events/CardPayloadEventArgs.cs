using System;

public class CardPayloadEventArgs<T> : EventArgs where T : CardPayload {
    private T cardPayload;

    public CardPayloadEventArgs(T cardPayload) {
        this.cardPayload = cardPayload;
    }

    public T CardPayload { get { return cardPayload; } }
}