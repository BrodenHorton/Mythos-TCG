using Unity.Netcode;

public class NullCardPayload : CardPayload {

    public NullCardPayload() {
        cardBaseIndex = -1;
        cardType = CardType.Null;
    }

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer) { }
}