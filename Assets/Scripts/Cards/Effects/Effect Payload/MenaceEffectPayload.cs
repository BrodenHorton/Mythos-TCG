using Unity.Netcode;

public class MenaceEffectPayload : StaticCreatureCardEffectPayload {

    public MenaceEffectPayload() {
        effectType = CreatureCardEffectType.Menace;
    }

    public MenaceEffectPayload(MenaceEffect effect) : base(effect) { }

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer) {
        serializer.SerializeValue(ref effectName);
        serializer.SerializeValue(ref description);
        serializer.SerializeValue(ref creatureUuidStr);
        serializer.SerializeValue(ref iconId);
    }
}