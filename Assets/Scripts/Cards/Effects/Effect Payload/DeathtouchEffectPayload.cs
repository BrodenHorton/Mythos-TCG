using Unity.Netcode;

public class DeathtouchEffectPayload : StaticCreatureCardEffectPayload {

    public DeathtouchEffectPayload() {
        effectType = CreatureCardEffectType.Deathtouch;
    }

    public DeathtouchEffectPayload(DeathtouchEffect effect) : base(effect) {
        effectType = CreatureCardEffectType.Deathtouch;
    }

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer) {
        serializer.SerializeValue(ref effectName);
        serializer.SerializeValue(ref rawDescription);
        serializer.SerializeValue(ref creatureUuidStr);
        serializer.SerializeValue(ref effecticonId);
    }
}
