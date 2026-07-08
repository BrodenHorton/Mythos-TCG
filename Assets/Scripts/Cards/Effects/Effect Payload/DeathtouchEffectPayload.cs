using Unity.Netcode;

public class DeathtouchEffectPayload : CreatureCardEffectPayload {

    public DeathtouchEffectPayload() {
        effectType = CreatureCardEffectType.Deathtouch;
    }

    public DeathtouchEffectPayload(DeathtouchEffect effect) {
        effectName = effect.EffectName;
        description = effect.Description;
        creatureUuidStr = effect.Card.Uuid.ToString();
        effectType = CreatureCardEffectType.Deathtouch;
    }

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer) {
        serializer.SerializeValue(ref effectName);
        serializer.SerializeValue(ref description);
        serializer.SerializeValue(ref creatureUuidStr);
    }
}
