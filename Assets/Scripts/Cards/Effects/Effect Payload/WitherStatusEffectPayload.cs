using Unity.Netcode;

public class WitherStatusEffectPayload : CreatureCardEffectPayload {
    private int witherCount;

    public WitherStatusEffectPayload() {
        effectType = CreatureCardEffectType.WitherStatus;
    }

    public WitherStatusEffectPayload(WitherStatusEffect effect) {
        creatureUuidStr = effect.Card.Uuid.ToString();
        effectType = CreatureCardEffectType.WitherStatus;
        witherCount = effect.WitherCount;
    }

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer) {
        serializer.SerializeValue(ref creatureUuidStr);
        serializer.SerializeValue(ref witherCount);
    }
}