using Unity.Collections;
using Unity.Netcode;

public abstract class CreatureCardEffectPayload : INetworkSerializable {
    protected FixedString128Bytes effectName;
    protected FixedString128Bytes rawDescription;
    protected FixedString128Bytes creatureUuidStr;
    protected CreatureCardEffectType effectType;

    public CreatureCardEffectPayload() { }

    public CreatureCardEffectPayload(CreatureCardEffect effect) {
        effectName = effect.GetCreatureEffectBase().EffectName;
        rawDescription = effect.GetRawDescription();
        creatureUuidStr = effect.Card.Uuid.ToString();
    }

    public abstract void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter;

    public FixedString128Bytes EffectName { get { return effectName; } }

    public FixedString128Bytes RawDescription { get { return rawDescription; } }

    public CreatureCardEffectType EffectType { get { return effectType; } }
}
