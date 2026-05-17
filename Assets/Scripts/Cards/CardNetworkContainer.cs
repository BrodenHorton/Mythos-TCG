using System;
using Unity.Netcode;

public struct CardNetworkContainer : INetworkSerializable {
    // Should be a deep copy so the original object doesn't get changed during serialization
    public Card card;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        CardType cardType = serializer.IsWriter && card != null ? card.CardType : default;
        serializer.SerializeValue(ref cardType);
        if(serializer.IsReader) {
            card = cardType switch {
                CardType.Creature => new CreatureCard(),
                CardType.Domain => new DomainCard(),
                CardType.Spell => new SpellCard(),
                CardType.Null => new NullCard(),
                _ => throw new NotImplementedException("Attempting to read card type that is not defined: " + cardType.ToString())
            };
        }
        card?.NetworkSerialize(serializer);
    }
}