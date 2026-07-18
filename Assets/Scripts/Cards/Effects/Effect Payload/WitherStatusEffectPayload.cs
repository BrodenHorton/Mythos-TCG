using Unity.Netcode;

public class WitherStatusEffectPayload : CreatureCardEffectPayload {
    private int witherCount;

    public WitherStatusEffectPayload() {
        effectType = CreatureCardEffectType.WitherStatus;
    }

    public WitherStatusEffectPayload(WitherStatusEffect effect) : base(effect) {
        witherCount = effect.WitherCount;
    }

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer) {
        serializer.SerializeValue(ref effectName);
        serializer.SerializeValue(ref description);
        serializer.SerializeValue(ref creatureUuidStr);
        serializer.SerializeValue(ref witherCount);
    }
}