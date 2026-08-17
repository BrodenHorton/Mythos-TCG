using Unity.Netcode;

public class SwarmAddEffectEffectPayload : CreatureCardEffectPayload {

    public SwarmAddEffectEffectPayload() : base() {
        effectType = CreatureCardEffectType.SwarmAddEffect;
    }

    public SwarmAddEffectEffectPayload(SwarmAddEffectEffect effect) : base(effect) {
        effectType = CreatureCardEffectType.SwarmAddEffect;
    }

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer) {
        serializer.SerializeValue(ref effectName);
        serializer.SerializeValue(ref rawDescription);
        serializer.SerializeValue(ref creatureUuidStr);
    }
}