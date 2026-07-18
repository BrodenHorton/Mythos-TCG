using Unity.Netcode;

public class WitherEffectPayload : StaticCreatureCardEffectPayload {

    public WitherEffectPayload() {
        effectType = CreatureCardEffectType.Wither;
    }

    public WitherEffectPayload(WitherEffect effect) : base(effect) { }

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer) {
        serializer.SerializeValue(ref effectName);
        serializer.SerializeValue(ref description);
        serializer.SerializeValue(ref creatureUuidStr);
        serializer.SerializeValue(ref iconId);
    }
}
