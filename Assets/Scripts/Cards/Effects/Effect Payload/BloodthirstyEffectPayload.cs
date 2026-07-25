using Unity.Netcode;

public class BloodthirstyEffectPayload : StaticCreatureCardEffectPayload {
    private int effectProkCount;

    public BloodthirstyEffectPayload() : base() {
        effectType = CreatureCardEffectType.Bloodthirsty;
    }

    public BloodthirstyEffectPayload(BloodthirstyEffect effect) : base(effect) {
        effectType = CreatureCardEffectType.Bloodthirsty;
        effectProkCount = effect.EffectProkCount;
    }

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer) {
        serializer.SerializeValue(ref effectName);
        serializer.SerializeValue(ref description);
        serializer.SerializeValue(ref creatureUuidStr);
        serializer.SerializeValue(ref iconId);
        serializer.SerializeValue(ref effectProkCount);
    }
}