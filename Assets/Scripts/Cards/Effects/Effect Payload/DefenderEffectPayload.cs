using Unity.Netcode;

public class DefenderEffectPayload : CreatureCardEffectPayload {

    public DefenderEffectPayload() {
        effectType = CreatureCardEffectType.Defender;
    }

    public DefenderEffectPayload(DefenderEffect effect) {
        effectName = effect.EffectName;
        description = effect.Description;
        creatureUuidStr = effect.Card.Uuid.ToString();
        effectType = CreatureCardEffectType.Defender;
    }

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer) {
        serializer.SerializeValue(ref effectName);
        serializer.SerializeValue(ref description);
        serializer.SerializeValue(ref creatureUuidStr);
    }
}