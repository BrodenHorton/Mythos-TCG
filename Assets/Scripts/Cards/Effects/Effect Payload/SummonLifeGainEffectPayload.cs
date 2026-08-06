using Unity.Netcode;

public class SummonLifeGainEffectPayload : CreatureCardEffectPayload {

    public SummonLifeGainEffectPayload() : base() {
        effectType = CreatureCardEffectType.SummonLifeGain;
    }

    public SummonLifeGainEffectPayload(SummonLifeGainEffect effect) : base(effect) {
        effectType = CreatureCardEffectType.SummonLifeGain;
    }

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer) {
        serializer.SerializeValue(ref effectName);
        serializer.SerializeValue(ref description);
        serializer.SerializeValue(ref creatureUuidStr);
    }
}