using Unity.Netcode;

public class DefenderEffectPayload : StaticCreatureCardEffectPayload {

    public DefenderEffectPayload() {
        effectType = CreatureCardEffectType.Defender;
    }

    public DefenderEffectPayload(DefenderEffect effect) : base(effect) { }

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer) {
        serializer.SerializeValue(ref effectName);
        serializer.SerializeValue(ref description);
        serializer.SerializeValue(ref creatureUuidStr);
        serializer.SerializeValue(ref iconId);
    }
}