using System;

[Serializable]
public class WitherStatusEffect : CreatureCardEffect {
    private static readonly string EFFECT_DESCRIPTION = "Wither status Description"; // TODO: Update later

    private WitherStatusEffectBase effectBase;
    private int witherCount;

    public WitherStatusEffect(WitherStatusEffectBase effectBase) {
        this.effectBase = effectBase;
    }

    public WitherStatusEffect(WitherStatusEffectBase effectBase, int witherCount) {
        this.effectBase = effectBase;
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
        return EFFECT_DESCRIPTION;
    }

    public override CreatureCardEffectBase GetCreatureEffectBase() {
        return effectBase;
    }

    public override CreatureCardEffectPayload GetEffectPayload() {
        return new WitherStatusEffectPayload(this);
    }

    public int WitherCount { get { return witherCount; } }
}
