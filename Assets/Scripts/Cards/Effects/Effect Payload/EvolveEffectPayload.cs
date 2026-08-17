using Unity.Netcode;

public class EvolveEffectPayload : CreatureCardEffectPayload {
    private int turnCount;

    public EvolveEffectPayload() : base() {
        effectType = CreatureCardEffectType.Evolve;
    }

    public EvolveEffectPayload(EvolveEffect effect) : base(effect) {
        effectType = CreatureCardEffectType.Evolve;
        turnCount = effect.TurnCount;
    }

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer) {
        serializer.SerializeValue(ref effectName);
        serializer.SerializeValue(ref rawDescription);
        serializer.SerializeValue(ref creatureUuidStr);
        serializer.SerializeValue(ref turnCount);
    }
}