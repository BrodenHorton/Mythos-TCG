using Unity.Netcode;

public class DuelistEffectPayload : StaticCreatureCardEffectPayload {

    public DuelistEffectPayload() {
        effectType = CreatureCardEffectType.Duelist;
    }

    public DuelistEffectPayload(DuelistEffect effect) : base(effect) { }

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer) {
        serializer.SerializeValue(ref effectName);
        serializer.SerializeValue(ref description);
        serializer.SerializeValue(ref creatureUuidStr);
        serializer.SerializeValue(ref iconId);
    }
}