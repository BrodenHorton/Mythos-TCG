using Unity.Netcode;

public class SwiftnessEffectPayload : CreatureCardEffectPayload {

    public SwiftnessEffectPayload() {
        effectType = CreatureCardEffectType.Swiftness;
    }

    public SwiftnessEffectPayload(SwiftnessEffect effect) {
        creatureUuidStr = effect.CreatureCardUuid.ToString();
        effectType = CreatureCardEffectType.Swiftness;
    }

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer) {
        serializer.SerializeValue(ref creatureUuidStr);
    }
}
