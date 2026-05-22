using System;
using Unity.Netcode;

[Serializable]
public abstract class CardPayload : INetworkSerializable {
    protected Guid uuid;
    protected CardType cardType;
    protected int manaCost;

    public abstract CardBase GetCardBase();

    public abstract void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter;

    public Guid Uuid { get { return uuid; } }

    public CardType CardType { get { return cardType; } }

    public int ManaCost { get { return manaCost; } }
}
