using Unity.Netcode;

public class CreatureCardPayload : CardPayload {
    private bool hasSummoningSickness;
    private bool isTapped;
    private int damage;
    private CreatureCardEffectPayloadNetworkContainer[] effectNetworkContainers;

    public CreatureCardPayload() {
        cardType = CardType.Creature;
    }

    public CreatureCardPayload(CreatureCard card) {
        uuidStr = card.Uuid.ToString();
        cardType = CardType.Creature;
        cardBaseIndex = CardDatabase.Instance.GetIndexOf(card.CardBase);
        hasSummoningSickness = card.HasSummoningSickness;
        isTapped = card.IsTapped;
        damage = card.CurrentDamage;
        effectNetworkContainers = new CreatureCardEffectPayloadNetworkContainer[card.Effects.Count];
        for(int i = 0; i < effectNetworkContainers.Length; i++) {
            CreatureCardEffectPayloadNetworkContainer effectNetworkContainer = new CreatureCardEffectPayloadNetworkContainer();
            effectNetworkContainer.effectPayload = card.Effects[i].GetEffectPayload();
            effectNetworkContainers[i] = effectNetworkContainer;
        }
    }

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer) {
        serializer.SerializeValue(ref uuidStr);
        serializer.SerializeValue(ref cardBaseIndex);
        serializer.SerializeValue(ref hasSummoningSickness);
        serializer.SerializeValue(ref isTapped);
        serializer.SerializeValue(ref damage);
        int effectCount = serializer.IsWriter ? effectNetworkContainers.Length : 0;
        serializer.SerializeValue(ref effectCount);
        if (serializer.IsReader)
            effectNetworkContainers = new CreatureCardEffectPayloadNetworkContainer[effectCount];
        for(int i = 0; i < effectCount; i++)
            serializer.SerializeNetworkSerializable(ref effectNetworkContainers[i]);
    }

    public bool HasSummoningSickness { get { return hasSummoningSickness; } }

    public bool IsTapped { get { return isTapped; } }

    public int Damage { get { return damage; } }

    public CreatureCardEffectPayloadNetworkContainer[] EffectContainers { get { return effectNetworkContainers; } }
}
