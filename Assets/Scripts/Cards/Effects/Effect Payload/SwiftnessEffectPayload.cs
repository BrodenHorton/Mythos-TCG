using Unity.Netcode;

public class SwiftnessEffectPayload : CreatureCardEffectPayload {

    public SwiftnessEffectPayload() {
        effectType = CreatureCardEffectType.Swiftness;
    }

    public SwiftnessEffectPayload(SwiftnessEffect effect) {
        effectName = effect.EffectName;
        description = effect.Description;
        creatureUuidStr = effect.Card.Uuid.ToString();
        effectType = CreatureCardEffectType.Swiftness;
    }

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer) {
        serializer.SerializeValue(ref effectName);
        serializer.SerializeValue(ref description);
        serializer.SerializeValue(ref creatureUuidStr);
    }
}
