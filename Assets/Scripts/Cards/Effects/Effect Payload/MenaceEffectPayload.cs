using Unity.Netcode;

public class MenaceEffectPayload : CreatureCardEffectPayload {

    public MenaceEffectPayload() {
        effectType = CreatureCardEffectType.Menace;
    }

    public MenaceEffectPayload(MenaceEffect effect) {
        creatureUuidStr = effect.CreatureCardUuid.ToString();
        effectType = CreatureCardEffectType.Menace;
    }

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer) {
        serializer.SerializeValue(ref creatureUuidStr);
    }
}