using Unity.Netcode;

public class ElusiveEffectPayload : StaticCreatureCardEffectPayload {

    public ElusiveEffectPayload() {
        effectType = CreatureCardEffectType.Elusive;
    }

    public ElusiveEffectPayload(ElusiveEffect effect) : base(effect) {
        effectType = CreatureCardEffectType.Elusive;
    }

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer) {
        serializer.SerializeValue(ref effectName);
        serializer.SerializeValue(ref description);
        serializer.SerializeValue(ref creatureUuidStr);
        serializer.SerializeValue(ref iconId);
    }
}
