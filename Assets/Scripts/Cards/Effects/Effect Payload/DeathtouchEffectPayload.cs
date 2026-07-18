using Unity.Netcode;

public class DeathtouchEffectPayload : StaticCreatureCardEffectPayload {

    public DeathtouchEffectPayload() {
        effectType = CreatureCardEffectType.Deathtouch;
    }

    public DeathtouchEffectPayload(DeathtouchEffect effect) : base(effect) { }

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer) {
        serializer.SerializeValue(ref effectName);
        serializer.SerializeValue(ref description);
        serializer.SerializeValue(ref creatureUuidStr);
        serializer.SerializeValue(ref iconId);
    }
}
