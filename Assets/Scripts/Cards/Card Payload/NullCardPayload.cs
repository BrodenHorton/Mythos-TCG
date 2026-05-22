using Unity.Netcode;

public class NullCardPayload : CardPayload {

    public NullCardPayload() {
        cardType = CardType.Null;
    }

    public override CardBase GetCardBase() {
        throw new System.NotImplementedException();
    }

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer) { }
}