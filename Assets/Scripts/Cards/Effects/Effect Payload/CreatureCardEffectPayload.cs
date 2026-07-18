using Unity.Collections;
using Unity.Netcode;

public abstract class CreatureCardEffectPayload : INetworkSerializable {
    protected FixedString128Bytes effectName;
    protected FixedString128Bytes description;
    protected FixedString128Bytes creatureUuidStr;
    protected CreatureCardEffectType effectType;

    public CreatureCardEffectPayload() { }

    public CreatureCardEffectPayload(CreatureCardEffect effect) {
        effectName = effect.EffectName;
        description = effect.GetFullDescription();
        creatureUuidStr = effect.Card.Uuid.ToString();
        effectType = CreatureCardEffectType.Bloodthirsty;
    }

    public abstract void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter;

    public FixedString128Bytes EffectName { get { return effectName; } }

    public FixedString128Bytes Description { get { return description; } }

    public CreatureCardEffectType EffectType { get { return effectType; } }
}
