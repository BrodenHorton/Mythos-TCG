using System;
using Unity.Collections;
using Unity.Netcode;

public struct CreatureCardNetworkSerializable : INetworkSerializable {
    public FixedString128Bytes uuidStr;
    public int cardBaseIndex;
    public bool hasSummoningSickness;
    public bool isTapped;
    public int damage;
    public CreatureCardEffectContainer effectContainer;

    public CreatureCardNetworkSerializable(FixedString128Bytes uuidStr,
                                           int cardBaseIndex,
                                           bool hasSummoningSickness,
                                           bool isTapped,
                                           int damage,
                                           CreatureCardEffectContainer effectContainer) {
        this.uuidStr = uuidStr;
        this.cardBaseIndex = cardBaseIndex;
        this.hasSummoningSickness = hasSummoningSickness;
        this.isTapped = isTapped;
        this.damage = damage;
        this.effectContainer = effectContainer;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        serializer.SerializeValue(ref uuidStr);
        serializer.SerializeValue(ref cardBaseIndex);
        serializer.SerializeValue(ref hasSummoningSickness);
        serializer.SerializeValue(ref isTapped);
        serializer.SerializeValue(ref damage);
        serializer.SerializeValue(ref effectContainer);
        serializer.SerializeNetworkSerializable(ref effectContainer);
    }
}