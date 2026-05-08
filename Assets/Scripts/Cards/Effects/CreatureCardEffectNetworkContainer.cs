using Unity.Netcode;

public struct CreatureCardEffectNetworkContainer : INetworkSerializable {
    public CreatureCardEffect[] effects;

    public CreatureCardEffectNetworkContainer(CreatureCardEffect[] effects) {
        this.effects = effects;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        int length = effects?.Length ?? 0;
        serializer.SerializeValue(ref length);

        if (serializer.IsReader)
            effects = new CreatureCardEffect[length];

        for (int i = 0; i < length; i++) {
            CreatureCardEffectType effectType = serializer.IsWriter ? effects[i].EffectType : default;
            serializer.SerializeValue(ref effectType);

            if (serializer.IsReader) {
                // Factory: Create the correct instance based on type tag
                effects[i] = effectType switch {
                    CreatureCardEffectType.Overwhelm => new OverwhelmEffect(),
                    _ => null
                };
            }
            effects[i]?.NetworkSerialize(serializer);
        }
    }
}
