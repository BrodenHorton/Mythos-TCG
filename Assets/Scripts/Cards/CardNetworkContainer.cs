using System;
using Unity.Netcode;

public struct CardNetworkContainer : INetworkSerializable {
    public Card card;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        string readerWriter = serializer.IsWriter ? "Writer" : "Reader";
        if(serializer.IsWriter) {
            TcgLogger.Log(readerWriter + ": is card null: " + (card == null));
            if (card != null)
                TcgLogger.Log(readerWriter + ": card uuid: " + card.Uuid);
        }
        CardType cardType = serializer.IsWriter ? card.CardType : CardType.Null;
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
        if (serializer.IsReader) {
            TcgLogger.Log(readerWriter + ": is card null: " + (card == null));
            if (card != null)
                TcgLogger.Log(readerWriter + ": card uuid: " + card.Uuid);
        }
        card.NetworkSerialize(serializer);
    }
}