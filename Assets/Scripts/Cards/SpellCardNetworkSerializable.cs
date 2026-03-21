using System;
using Unity.Collections;
using Unity.Netcode;

public struct SpellCardNetworkSerializable : INetworkSerializable {
    public FixedString128Bytes uuidStr;
    public int cardBaseIndex;

    public SpellCardNetworkSerializable(FixedString128Bytes uuidStr, int cardBaseIndex) {
        this.uuidStr = uuidStr;
        this.cardBaseIndex = cardBaseIndex;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        serializer.SerializeValue(ref uuidStr);
        serializer.SerializeValue(ref cardBaseIndex);
    }
}