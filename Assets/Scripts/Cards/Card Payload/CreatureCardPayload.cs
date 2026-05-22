using System;
using Unity.Collections;
using Unity.Netcode;

public class CreatureCardPayload : CardPayload {
    private CreatureCardBase cardBase;
    private int atk;
    private int health;
    private bool hasSummoningSickness;
    private bool isTapped;
    private int damage;
    private CreatureCardEffectPayloadNetworkContainer[] effectNetworkContainers;

    public CreatureCardPayload() {
        cardType = CardType.Creature;
    }

    public CreatureCardPayload(CreatureCard card) {
        uuid = card.Uuid;
        cardType = CardType.Creature;
        cardBase = card.CardBase;
        manaCost = card.GetManaCost();
        atk = card.GetAtk();
        health = card.GetHealth();
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

    public override CardBase GetCardBase() {
        return cardBase;
    }

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer) {
        FixedString128Bytes uuidStr = serializer.IsWriter ? uuid.ToString() : "";
        serializer.SerializeValue(ref uuidStr);
        if (serializer.IsReader)
            uuid = Guid.Parse(uuidStr.ToString());
        FixedString128Bytes cardBaseId = serializer.IsWriter ? cardBase.Id : "";
        serializer.SerializeValue(ref cardBaseId);
        if (serializer.IsReader)
            cardBase = CardDatabase.Instance.GetCreatureCardById(cardBaseId.ToString());
        serializer.SerializeValue(ref manaCost);
        serializer.SerializeValue(ref atk);
        serializer.SerializeValue(ref health);
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

    public CreatureCardBase CardBase { get { return cardBase; } }

    public int Atk { get { return atk; } }

    public int Health { get { return health; } }

    public bool HasSummoningSickness { get { return hasSummoningSickness; } }

    public bool IsTapped { get { return isTapped; } }

    public int Damage { get { return damage; } }

    public CreatureCardEffectPayloadNetworkContainer[] EffectContainers { get { return effectNetworkContainers; } }
}
