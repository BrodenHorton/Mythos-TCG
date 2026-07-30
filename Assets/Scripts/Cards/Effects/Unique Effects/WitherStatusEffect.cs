using System;

[Serializable]
public class WitherStatusEffect : CreatureCardEffect {
    private static readonly string EFFECT_NAME = "Wither Status";
    private static readonly string EFFECT_DESCRIPTION = ""; // TODO: Dynamically update with current wither count

    private int witherCount;

    public WitherStatusEffect() : this(0) { }

    public WitherStatusEffect(WitherStatusEffect effect) : this(effect.witherCount) { }

    public WitherStatusEffect(int witherCount) : base() {
        effectName = EFFECT_NAME;
        description = EFFECT_DESCRIPTION;
        this.witherCount = witherCount;
    }

    public override void Init(CreatureCard card) {
        this.card = card;
        EventBus.Instance.OnWitherProked += AddEffectProk;
        EventBus.Instance.OnCalculateCreatureAttack += UpdateAttack;
        EventBus.Instance.OnCalculateCreatureHealth += UpdateHealth;
    }

    public override void RemoveListeners() {
        EventBus.Instance.OnWitherProked -= AddEffectProk;
        EventBus.Instance.OnCalculateCreatureAttack -= UpdateAttack;
        EventBus.Instance.OnCalculateCreatureHealth -= UpdateHealth;
    }

    private void AddEffectProk(object sender, CreatureCombatDamageEventArgs args) {
        if (args.Defender?.Uuid != card.Uuid)
            return;

        args.IsCanceled = true;
        witherCount += args.Damage;
        TcgLogger.Log("Wither Status Proked. Count: " + witherCount);
        card.CheckHealthState();
    }

    private void UpdateAttack(object sender, PlayerCardStatEventArgs<CreatureCard> args) {
        if (args.Card.Uuid != card.Uuid)
            return;

        args.Value -= witherCount;
    }

    private void UpdateHealth(object sender, PlayerCardStatEventArgs<CreatureCard> args) {
        if (args.Card.Uuid != card.Uuid)
            return;

        args.Value -= witherCount;
    }

    public override string GetFullDescription() {
        return description;
    }

    public override CreatureCardEffect DeepCopy() {
        return new WitherStatusEffect(this);
    }

    public override CreatureCardEffectPayload GetEffectPayload() {
        return new WitherStatusEffectPayload(this);
    }

    public int WitherCount { get { return witherCount; } }
}
