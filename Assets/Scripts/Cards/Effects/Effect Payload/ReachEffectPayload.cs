using Unity.Netcode;

public class ReachEffectPayload : CreatureCardEffectPayload {

    public ReachEffectPayload() {
        effectType = CreatureCardEffectType.Reach;
    }

    public ReachEffectPayload(ReachEffect effect) {
        creatureUuidStr = effect.Card.Uuid.ToString();
        effectType = CreatureCardEffectType.Reach;
    }

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer) {
        serializer.SerializeValue(ref creatureUuidStr);
    }
}