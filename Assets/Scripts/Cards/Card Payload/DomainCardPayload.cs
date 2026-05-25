using System;
using Unity.Collections;
using Unity.Netcode;

public class DomainCardPayload : CardPayload {
    private DomainCardBase cardBase;

    public DomainCardPayload() {
        cardType = CardType.Domain;
    }

    public DomainCardPayload(DomainCard card) {
        cardType = CardType.Domain;
        uuid = card.Uuid;
        manaCost = card.GetManaCost();
        cardBase = card.CardBase;
    }

    public override CardBase GetCardBase() {
        return cardBase;
    }

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer) {
        FixedString128Bytes uuidStr = serializer.IsWriter ? uuid.ToString() : "";
        serializer.SerializeValue(ref uuidStr);
        if (serializer.IsReader)
            uuid = Guid.Parse(uuidStr.ToString());
        serializer.SerializeValue(ref manaCost);
        FixedString128Bytes cardBaseId = serializer.IsWriter ? cardBase.Id : "";
        serializer.SerializeValue(ref cardBaseId);
        if (serializer.IsReader)
            cardBase = CardDatabase.Instance.GetDomainCardById(cardBaseId.ToString());
    }

    public DomainCardBase CardBase { get { return cardBase; } }
}
