using Unity.Netcode;

public class BattleCryStatBoostEffectPayload : CreatureCardEffectPayload {
    private int effectProkCount;

    public BattleCryStatBoostEffectPayload() : base() {
        effectType = CreatureCardEffectType.BattleCryStatBoost;
    }

    public BattleCryStatBoostEffectPayload(BattleCryStatBoostEffect effect) : base(effect) {
        effectType = CreatureCardEffectType.BattleCryStatBoost;
        effectProkCount = effect.EffectProkCount;
    }

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer) {
        serializer.SerializeValue(ref effectName);
        serializer.SerializeValue(ref rawDescription);
        serializer.SerializeValue(ref creatureUuidStr);
        serializer.SerializeValue(ref effectProkCount);
    }
}
