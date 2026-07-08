using Unity.Netcode;

public class MenaceEffectPayload : CreatureCardEffectPayload {

    public MenaceEffectPayload() {
        effectType = CreatureCardEffectType.Menace;
    }

    public MenaceEffectPayload(MenaceEffect effect) {
        effectName = effect.EffectName;
        description = effect.Description;
        creatureUuidStr = effect.Card.Uuid.ToString();
        effectType = CreatureCardEffectType.Menace;
    }

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer) {
        serializer.SerializeValue(ref effectName);
        serializer.SerializeValue(ref description);
        serializer.SerializeValue(ref creatureUuidStr);
    }
}