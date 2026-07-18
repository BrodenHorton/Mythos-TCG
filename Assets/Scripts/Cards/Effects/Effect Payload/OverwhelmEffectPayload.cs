using Unity.Netcode;

public class OverwhelmEffectPayload : StaticCreatureCardEffectPayload {
    
    public OverwhelmEffectPayload() {
        effectType = CreatureCardEffectType.Overwhelm;
    }

    public OverwhelmEffectPayload(OverwhelmEffect effect) : base(effect) { }

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer) {
        serializer.SerializeValue(ref effectName);
        serializer.SerializeValue(ref description);
        serializer.SerializeValue(ref creatureUuidStr);
        serializer.SerializeValue(ref iconId);
    }
}
