using Unity.Netcode;

public class ElusiveEffectPayload : CreatureCardEffectPayload {

    public ElusiveEffectPayload() {
        effectType = CreatureCardEffectType.Elusive;
    }

    public ElusiveEffectPayload(ElusiveEffect effect) {
        creatureUuidStr = effect.Card.Uuid.ToString();
        effectType = CreatureCardEffectType.Elusive;
    }

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer) {
        serializer.SerializeValue(ref creatureUuidStr);
    }
}
