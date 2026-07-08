using Unity.Netcode;

public class DuelistEffectPayload : CreatureCardEffectPayload {

    public DuelistEffectPayload() {
        effectType = CreatureCardEffectType.Duelist;
    }

    public DuelistEffectPayload(DuelistEffect effect) {
        effectName = effect.EffectName;
        description = effect.Description;
        creatureUuidStr = effect.Card.Uuid.ToString();
        effectType = CreatureCardEffectType.Duelist;
    }

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer) {
        serializer.SerializeValue(ref effectName);
        serializer.SerializeValue(ref description);
        serializer.SerializeValue(ref creatureUuidStr);
    }
}