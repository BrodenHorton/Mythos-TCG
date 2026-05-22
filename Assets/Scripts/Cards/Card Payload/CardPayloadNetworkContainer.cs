using System;
using Unity.Netcode;

public struct CardPayloadNetworkContainer : INetworkSerializable {
    public CardPayload cardPayload;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        CardType cardType = serializer.IsWriter && cardPayload != null ? cardPayload.CardType : default;
        serializer.SerializeValue(ref cardType);
        if (serializer.IsReader) {
            cardPayload = cardType switch {
                CardType.Creature => new CreatureCardPayload(),
                CardType.Domain => new DomainCardPayload(),
                CardType.Spell => new SpellCardPayload(),
                CardType.Null => new NullCardPayload(),
                _ => throw new NotImplementedException("Attempting to read card type that is not defined: " + cardType.ToString())
            };
        }
        cardPayload?.NetworkSerialize(serializer);
    }
}
