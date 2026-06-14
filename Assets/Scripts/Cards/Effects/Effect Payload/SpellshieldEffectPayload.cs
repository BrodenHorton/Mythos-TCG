using Unity.Netcode;

public class SpellshieldEffectPayload : CreatureCardEffectPayload {

    public SpellshieldEffectPayload() {
        effectType = CreatureCardEffectType.Spellshield;
    }

    public SpellshieldEffectPayload(SpellshieldEffect effect) {
        creatureUuidStr = effect.Card.Uuid.ToString();
        effectType = CreatureCardEffectType.Spellshield;
    }

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer) {
        serializer.SerializeValue(ref creatureUuidStr);
    }
}
