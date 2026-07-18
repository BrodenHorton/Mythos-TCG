using System;

[Serializable]
public class OverwhelmEffect : StaticCreatureCardEffect {
    private static readonly string EFFECT_NAME = "Overwhelm";
    private static readonly string EFFECT_DESCRIPTION = "Overflow damage that isn’t blocked by a defender's Health is dealt as life point damage.";

    public OverwhelmEffect() : base() {
        effectName = EFFECT_NAME;
        description = EFFECT_DESCRIPTION;
        effectIconId = "";
    }

    public OverwhelmEffect(OverwhelmEffect effect) : this() { }

    public override void Init(CreatureCard card) {
        this.card = card;
        EventBus.Instance.OnCreatureDamagedByCreature += DealOverwhelmDamage;
    }

    public override void RemoveListeners() {
        EventBus.Instance.OnCreatureDamagedByCreature -= DealOverwhelmDamage;
    }

    private void DealOverwhelmDamage(object sender, CreatureCombatDamageEventArgs args) {
        if (args.Attacker.Uuid != card.Uuid)
            return;
        if (args.IsCanceled)
            return;

        TcgLogger.Log("Overwhelm Effect triggered");
        int overwhelmDamage = 0;
        if (args.Defender == null)
            overwhelmDamage = args.Damage;
        else if (args.Defender.GetHealth() < args.Damage)
            overwhelmDamage = args.Damage - args.Defender.GetHealth();

        if (overwhelmDamage > 0)
            args.DirectDamage = overwhelmDamage;
    }

    public override string GetFullDescription() {
        return description;
    }

    public override CreatureCardEffect DeepCopy() {
        return new OverwhelmEffect(this);
    }

    public override CreatureCardEffectPayload GetEffectPayload() {
        return new OverwhelmEffectPayload(this);
    }
}
