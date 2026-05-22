using Unity.Netcode;

public class DomainCardPayload : CardPayload {

    public DomainCardPayload() {
        cardType = CardType.Domain;
    }

    public DomainCardPayload(DomainCard card) {
        uuidStr = card.Uuid.ToString();
        cardType = CardType.Domain;
        cardBaseIndex = CardDatabase.Instance.GetIndexOf(card.CardBase);
    }

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer) {
        serializer.SerializeValue(ref uuidStr);
        serializer.SerializeValue(ref cardBaseIndex);
    }
}
