using System;

[Serializable]
public class BlessingStatBoostEffect : BlessingEffect {
    private BlessingStatBoostEffectBase effectBase;
    private int effectProkCount;

    public BlessingStatBoostEffect(BlessingStatBoostEffectBase effectBase) : base(effectBase) {
        this.effectBase = effectBase;
        effectProkCount = 0;
    }

    public override void Init(CreatureCard card) {
        this.card = card;
        EventBus.Instance.OnLifePointsChanged += BlessingEffectHandler;
        EventBus.Instance.OnCalculateCreatureAttack += AddAttack;
        EventBus.Instance.OnCalculateCreatureHealth += AddHealth;
    }

    public override void RemoveListeners() {
        EventBus.Instance.OnLifePointsChanged -= BlessingEffectHandler;
        EventBus.Instance.OnCalculateCreatureAttack -= AddAttack;
        EventBus.Instance.OnCalculateCreatureHealth -= AddHealth;
    }

    protected override void BlessingEffectHandler(object sender, LifePointsChangedEventArgs args) {
        if (args.PlayerId != card.PlayerId)
            return;
        if (args.PreviousLifePoints >= args.LifePoints)
            return;

        TcgLogger.Log("BlessingStatBoostEffect Proked");
        effectProkCount++;
    }

    private void AddAttack(object sender, PlayerCardStatEventArgs<CreatureCard> args) {
        if (args.Card.Uuid != card.Uuid)
            return;
        if (effectProkCount <= 0 || effectBase.AtkBoost <= 0)
            return;

        args.Value += effectProkCount * effectBase.AtkBoost;
    }

    private void AddHealth(object sender, PlayerCardStatEventArgs<CreatureCard> args) {
        if (args.Card.Uuid != card.Uuid)
            return;
        if (effectProkCount <= 0 || effectBase.HealthBoost <= 0)
            return;

        args.Value += effectProkCount * effectBase.HealthBoost;
    }

    public override CreatureCardEffectBase GetCreatureEffectBase() {
        return effectBase;
    }

    public override CreatureCardEffectPayload GetEffectPayload() {
        return new BlessingStatBoostEffectPayload(this);
    }

    public int EffectProkCount { get { return effectProkCount; } }
}