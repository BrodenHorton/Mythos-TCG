using Unity.Netcode;

public class StaticCreatureCardEffectPayload : CreatureCardEffectPayload {
    protected string effecticonId;

    public StaticCreatureCardEffectPayload() { }

    public StaticCreatureCardEffectPayload(StaticCreatureCardEffect effect) : base(effect) {
        effecticonId = effect.EffectIconId;
    }

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer) {
        serializer.SerializeValue(ref rawDescription);
        serializer.SerializeValue(ref creatureUuidStr);
        serializer.SerializeValue(ref effecticonId);
    }

    public string EffectIconId { get { return effecticonId; } }
}