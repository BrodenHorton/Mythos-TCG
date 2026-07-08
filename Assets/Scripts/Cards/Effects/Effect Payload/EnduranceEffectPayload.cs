using Unity.Netcode;

public class EnduranceEffectPayload : CreatureCardEffectPayload {

    public EnduranceEffectPayload() {
        effectType = CreatureCardEffectType.Endurance;
    }

    public EnduranceEffectPayload(EnduranceEffect effect) {
        effectName = effect.EffectName;
        description = effect.Description;
        creatureUuidStr = effect.Card.Uuid.ToString();
        effectType = CreatureCardEffectType.Endurance;
    }

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer) {
        serializer.SerializeValue(ref effectName);
        serializer.SerializeValue(ref description);
        serializer.SerializeValue(ref creatureUuidStr);
    }
}