using Unity.Netcode;

public class LifelinkEffectPayload : CreatureCardEffectPayload {

    public LifelinkEffectPayload() {
        effectType = CreatureCardEffectType.Lifelink;
    }

    public LifelinkEffectPayload(LifelinkEffect effect) {
        creatureUuidStr = effect.Card.Uuid.ToString();
        effectType = CreatureCardEffectType.Lifelink;
    }

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer) {
        serializer.SerializeValue(ref creatureUuidStr);
    }
}