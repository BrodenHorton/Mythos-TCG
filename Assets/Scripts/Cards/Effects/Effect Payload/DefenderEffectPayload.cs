using Unity.Netcode;

public class DefenderEffectPayload : CreatureCardEffectPayload {

    public DefenderEffectPayload() {
        effectType = CreatureCardEffectType.Defender;
    }

    public DefenderEffectPayload(DefenderEffect effect) {
        creatureUuidStr = effect.CreatureCardUuid.ToString();
        effectType = CreatureCardEffectType.Defender;
    }

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer) {
        serializer.SerializeValue(ref creatureUuidStr);
    }
}