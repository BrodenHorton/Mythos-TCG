using System;
using Unity.Collections;
using Unity.Netcode;

public class SpellCardPayload : CardPayload {
    private SpellCardBase cardBase;

    public SpellCardPayload() {
        cardType = CardType.Spell;
    }

    public SpellCardPayload(SpellCard card) {
        uuid = card.Uuid;
        cardType = CardType.Spell;
    }

    public override CardBase GetCardBase() {
        return cardBase;
    }

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer) {
        FixedString128Bytes uuidStr = serializer.IsWriter ? uuid.ToString() : "";
        serializer.SerializeValue(ref uuidStr);
        if (serializer.IsReader)
            uuid = Guid.Parse(uuidStr.ToString());
        FixedString128Bytes cardBaseId = serializer.IsWriter ? cardBase.Id : "";
        serializer.SerializeValue(ref cardBaseId);
        if (serializer.IsReader)
            cardBase = CardDatabase.Instance.GetSpellCardById(cardBaseId.ToString());
    }

    public SpellCardBase CardBase { get { return cardBase; } }
}
