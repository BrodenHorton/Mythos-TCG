using Unity.Netcode;

public class DeathCryCardSearchEffectPayload : CreatureCardEffectPayload {

    public DeathCryCardSearchEffectPayload() : base() {
        effectType = CreatureCardEffectType.DeathCryCardSearch;
    }

    public DeathCryCardSearchEffectPayload(DeathCryCardSearchEffect effect) : base(effect) {
        effectType = CreatureCardEffectType.DeathCryCardSearch;
    }

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer) {
        serializer.SerializeValue(ref effectName);
        serializer.SerializeValue(ref rawDescription);
        serializer.SerializeValue(ref creatureUuidStr);
    }
}