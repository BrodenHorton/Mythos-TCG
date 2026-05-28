using Unity.Netcode;

public class BloodthirstyEffectPayload : CreatureCardEffectPayload {
    private int effectProkCount;

    public BloodthirstyEffectPayload() {
        effectType = CreatureCardEffectType.Bloodthirsty;
    }

    public BloodthirstyEffectPayload(BloodthirstyEffect effect) {
        creatureUuidStr = effect.CreatureCardUuid.ToString();
        effectType = CreatureCardEffectType.Bloodthirsty;
        effectProkCount = effect.EffectProkCount;
    }

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer) {
        serializer.SerializeValue(ref creatureUuidStr);
        serializer.SerializeValue(ref effectProkCount);
    }
}