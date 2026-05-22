using Unity.Collections;
using Unity.Netcode;

public abstract class CreatureCardEffectPayload : INetworkSerializable {
    protected FixedString128Bytes creatureUuidStr;
    protected CreatureCardEffectType effectType;

    public abstract void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter;

    public FixedString128Bytes CreatureUuidStr { get { return creatureUuidStr; } }

    public CreatureCardEffectType EffectType { get { return effectType; } }
}
