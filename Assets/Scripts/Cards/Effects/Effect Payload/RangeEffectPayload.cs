using Unity.Netcode;

public class RangeEffectPayload : CreatureCardEffectPayload {

    public RangeEffectPayload() {
        effectType = CreatureCardEffectType.Range;
    }

    public RangeEffectPayload(RangeEffect effect) {
        creatureUuidStr = effect.CreatureCardUuid.ToString();
        effectType = CreatureCardEffectType.Range;
    }

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer) {
        serializer.SerializeValue(ref creatureUuidStr);
    }
}