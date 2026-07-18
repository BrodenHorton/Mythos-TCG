using Unity.Netcode;

public class LifelinkEffectPayload : StaticCreatureCardEffectPayload {

    public LifelinkEffectPayload() {
        effectType = CreatureCardEffectType.Lifelink;
    }

    public LifelinkEffectPayload(LifelinkEffect effect) : base(effect) { }

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer) {
        serializer.SerializeValue(ref effectName);
        serializer.SerializeValue(ref description);
        serializer.SerializeValue(ref creatureUuidStr);
        serializer.SerializeValue(ref iconId);
    }
}