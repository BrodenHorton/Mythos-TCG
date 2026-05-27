using Unity.Netcode;

public class EnduranceEffectPayload : CreatureCardEffectPayload {

    public EnduranceEffectPayload() {
        effectType = CreatureCardEffectType.Endurance;
    }

    public EnduranceEffectPayload(EnduranceEffect effect) {
        creatureUuidStr = effect.CreatureCardUuid.ToString();
        effectType = CreatureCardEffectType.Endurance;
    }

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer) {
        serializer.SerializeValue(ref creatureUuidStr);
    }
}