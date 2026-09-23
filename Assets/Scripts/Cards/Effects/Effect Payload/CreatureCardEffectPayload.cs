using Unity.Collections;
using Unity.Netcode;

public class CreatureCardEffectPayload : INetworkSerializable {
    protected FixedString512Bytes rawDescription;
    protected FixedString128Bytes creatureUuidStr;

    public CreatureCardEffectPayload() { }

    public CreatureCardEffectPayload(CardEffect<CreatureCard> effect) {
        rawDescription = effect.GetRawDescription();
        creatureUuidStr = effect.Card.Uuid.ToString();
    }

    public virtual void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        serializer.SerializeValue(ref rawDescription);
        serializer.SerializeValue(ref creatureUuidStr);
    }

    public FixedString512Bytes RawDescription { get { return rawDescription; } }
}
