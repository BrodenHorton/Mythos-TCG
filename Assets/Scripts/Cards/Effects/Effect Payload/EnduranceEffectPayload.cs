using Unity.Netcode;

public class EnduranceEffectPayload : StaticCreatureCardEffectPayload {

    public EnduranceEffectPayload() {
        effectType = CreatureCardEffectType.Endurance;
    }

    public EnduranceEffectPayload(EnduranceEffect effect) : base(effect) { }

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer) {
        serializer.SerializeValue(ref effectName);
        serializer.SerializeValue(ref description);
        serializer.SerializeValue(ref creatureUuidStr);
        serializer.SerializeValue(ref iconId);
    }
}