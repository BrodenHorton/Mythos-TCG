using Unity.Netcode;

public class SwiftnessEffectPayload : StaticCreatureCardEffectPayload {

    public SwiftnessEffectPayload() {
        effectType = CreatureCardEffectType.Swiftness;
    }

    public SwiftnessEffectPayload(SwiftnessEffect effect) : base(effect) {
        effectType = CreatureCardEffectType.Swiftness;
    }

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer) {
        serializer.SerializeValue(ref effectName);
        serializer.SerializeValue(ref description);
        serializer.SerializeValue(ref creatureUuidStr);
        serializer.SerializeValue(ref iconId);
    }
}
