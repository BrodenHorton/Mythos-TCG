using Unity.Netcode;

public class SpellCardPayload : CardPayload {

    public SpellCardPayload() {
        cardType = CardType.Spell;
    }

    public SpellCardPayload(SpellCard card) {
        uuidStr = card.Uuid.ToString();
        cardType = CardType.Spell;
        cardBaseIndex = CardDatabase.Instance.GetIndexOf(card.CardBase);
    }

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer) {
        serializer.SerializeValue(ref uuidStr);
        serializer.SerializeValue(ref cardBaseIndex);
    }
}
