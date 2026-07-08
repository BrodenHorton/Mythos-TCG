using Unity.Netcode;

public class ElusiveEffectPayload : CreatureCardEffectPayload {

    public ElusiveEffectPayload() {
        effectType = CreatureCardEffectType.Elusive;
    }

    public ElusiveEffectPayload(ElusiveEffect effect) {
        effectName = effect.EffectName;
        description = effect.Description;
        creatureUuidStr = effect.Card.Uuid.ToString();
        effectType = CreatureCardEffectType.Elusive;
    }

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer) {
        serializer.SerializeValue(ref effectName);
        serializer.SerializeValue(ref description);
        serializer.SerializeValue(ref creatureUuidStr);
    }
}
