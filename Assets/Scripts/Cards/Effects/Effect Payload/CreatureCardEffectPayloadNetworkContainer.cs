using System;
using Unity.Netcode;

public struct CreatureCardEffectPayloadNetworkContainer : INetworkSerializable {
    public CreatureCardEffectPayload effectPayload;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        CreatureCardEffectType effectType = serializer.IsWriter && effectPayload != null ? effectPayload.EffectType : default;
        serializer.SerializeValue(ref effectType);
        if (serializer.IsReader) {
            effectPayload = effectType switch {
                CreatureCardEffectType.Overwhelm => new OverwhelmEffectPayload(),
                CreatureCardEffectType.Elusive => new ElusiveEffectPayload(),
                CreatureCardEffectType.Reach => new ReachEffectPayload(),
                _ => throw new NotImplementedException("Attempting to read card type that is not defined: " + effectType.ToString())
            };
        }
        effectPayload?.NetworkSerialize(serializer);
    }
}
