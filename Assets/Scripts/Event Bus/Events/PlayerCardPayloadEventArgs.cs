using System;

public class PlayerCardPayloadEventArgs<T> : EventArgs where T : CardPayload {
    private ulong playerId;
    private T cardPayload;

    public PlayerCardPayloadEventArgs(ulong playerId, T cardPayload) {
        this.playerId = playerId;
        this.cardPayload = cardPayload;
    }

    public ulong PlayerId { get { return playerId; } }

    public T CardPayload { get { return cardPayload; } }
}
