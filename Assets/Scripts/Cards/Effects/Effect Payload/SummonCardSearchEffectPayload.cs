using Unity.Netcode;

public class SummonCardSearchEffectPayload : CreatureCardEffectPayload {

    public SummonCardSearchEffectPayload() : base() {
        effectType = CreatureCardEffectType.SummonCardSearch;
    }

    public SummonCardSearchEffectPayload(SummonCardSearchEffect effect) : base(effect) {
        effectType = CreatureCardEffectType.SummonCardSearch;
    }

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer) {
        serializer.SerializeValue(ref effectName);
        serializer.SerializeValue(ref rawDescription);
        serializer.SerializeValue(ref creatureUuidStr);
    }
}