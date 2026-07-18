using Unity.Netcode;

public class ReachEffectPayload : StaticCreatureCardEffectPayload {

    public ReachEffectPayload() {
        effectType = CreatureCardEffectType.Reach;
    }

    public ReachEffectPayload(ReachEffect effect) : base(effect) { }

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer) {
        serializer.SerializeValue(ref effectName);
        serializer.SerializeValue(ref description);
        serializer.SerializeValue(ref creatureUuidStr);
        serializer.SerializeValue(ref iconId);
    }
}