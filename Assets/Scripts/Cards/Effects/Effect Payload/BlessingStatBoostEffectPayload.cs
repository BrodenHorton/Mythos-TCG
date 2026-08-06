using Unity.Netcode;

public class BlessingStatBoostEffectPayload : CreatureCardEffectPayload {
    private int effectProkCount;

    public BlessingStatBoostEffectPayload() : base() {
        effectType = CreatureCardEffectType.BlessingStatBoost;
    }

    public BlessingStatBoostEffectPayload(BlessingStatBoostEffect effect) : base(effect) {
        effectType = CreatureCardEffectType.BlessingStatBoost;
        effectProkCount = effect.EffectProkCount;
    }

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer) {
        serializer.SerializeValue(ref effectName);
        serializer.SerializeValue(ref description);
        serializer.SerializeValue(ref creatureUuidStr);
        serializer.SerializeValue(ref effectProkCount);
    }
}
