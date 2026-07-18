using Unity.Netcode;

public class SpellshieldEffectPayload : StaticCreatureCardEffectPayload {

    public SpellshieldEffectPayload() {
        effectType = CreatureCardEffectType.Spellshield;
    }

    public SpellshieldEffectPayload(SpellshieldEffect effect) : base(effect) { }

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer) {
        serializer.SerializeValue(ref effectName);
        serializer.SerializeValue(ref description);
        serializer.SerializeValue(ref creatureUuidStr);
        serializer.SerializeValue(ref iconId);
    }
}
