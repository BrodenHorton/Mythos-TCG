using Unity.Netcode;

public class WitherEffectPayload : CreatureCardEffectPayload {

    public WitherEffectPayload() {
        effectType = CreatureCardEffectType.Wither;
    }

    public WitherEffectPayload(WitherEffect effect) {
        creatureUuidStr = effect.Card.Uuid.ToString();
        effectType = CreatureCardEffectType.Wither;
    }

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer) {
        serializer.SerializeValue(ref creatureUuidStr);
    }
}
