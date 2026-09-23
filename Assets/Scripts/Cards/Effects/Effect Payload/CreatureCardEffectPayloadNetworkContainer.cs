using System;
using Unity.Netcode;

public struct CreatureCardEffectPayloadNetworkContainer : INetworkSerializable {
    public enum CreatureCardEffectPayloadType {
        Normal,
        Static
    }

    public CreatureCardEffectPayload effectPayload;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        CreatureCardEffectPayloadType effectType = default;
        if(serializer.IsWriter && effectPayload != null)
            effectType = effectPayload is StaticCreatureCardEffectPayload ? CreatureCardEffectPayloadType.Static : CreatureCardEffectPayloadType.Normal;

        serializer.SerializeValue(ref effectType);
        if (serializer.IsReader) {
            effectPayload = effectType switch {
                CreatureCardEffectPayloadType.Normal => new CreatureCardEffectPayload(),
                CreatureCardEffectPayloadType.Static => new StaticCreatureCardEffectPayload(),
                _ => throw new NotImplementedException("Attempting to read card effect type that is not defined: " + effectType.ToString())
            };
        }
        effectPayload?.NetworkSerialize(serializer);
    }
}