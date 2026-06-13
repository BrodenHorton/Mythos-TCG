using Unity.Netcode;

public class OverwhelmEffectPayload : CreatureCardEffectPayload {
    
    public OverwhelmEffectPayload() {
        effectType = CreatureCardEffectType.Overwhelm;
    }

    public OverwhelmEffectPayload(OverwhelmEffect effect) {
        creatureUuidStr = effect.Card.Uuid.ToString();
        effectType = CreatureCardEffectType.Overwhelm;
    }

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer) {
        serializer.SerializeValue(ref creatureUuidStr);
    }
}
