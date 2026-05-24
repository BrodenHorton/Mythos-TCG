using System;
using Unity.Netcode;

[Serializable]
public abstract class CreatureCardEffect : INetworkSerializable {
    protected Guid creatureCardUuid;

    public CreatureCardEffect() { }

    public abstract void Init(Guid creatureCardUuid);

    public abstract void RemoveListeners();

    public abstract CreatureCardEffect DeepCopy();

    public abstract CreatureCardEffectPayload GetEffectPayload();

    public abstract void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter;

    public Guid CreatureCardUuid { get { return creatureCardUuid; } }
}
