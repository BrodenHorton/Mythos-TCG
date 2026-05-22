using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

[Serializable]
public abstract class CardPayload : INetworkSerializable {
    protected FixedString128Bytes uuidStr;
    protected CardType cardType;
    protected int cardBaseIndex;

    public abstract void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter;

    public FixedString128Bytes UuidStr { get { return uuidStr; } }

    public CardType CardType { get { return cardType; } }

    public int CardBaseIndex { get { return cardBaseIndex; } }
}
