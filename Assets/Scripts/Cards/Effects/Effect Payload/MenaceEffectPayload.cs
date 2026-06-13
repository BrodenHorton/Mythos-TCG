using Unity.Netcode;

public class MenaceEffectPayload : CreatureCardEffectPayload {

    public MenaceEffectPayload() {
        effectType = CreatureCardEffectType.Menace;
    }

    public MenaceEffectPayload(MenaceEffect effect) {
        creatureUuidStr = effect.Card.Uuid.ToString();
        effectType = CreatureCardEffectType.Menace;
    }

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer) {
        serializer.SerializeValue(ref creatureUuidStr);
    }
}