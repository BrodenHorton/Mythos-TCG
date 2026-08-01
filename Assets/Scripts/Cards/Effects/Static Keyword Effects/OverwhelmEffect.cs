using System;

[Serializable]
public class OverwhelmEffect : StaticCreatureCardEffect {
    private OverwhelmEffectBase effectBase;

    public OverwhelmEffect(OverwhelmEffectBase effectBase) {
        this.effectBase = effectBase;
    }

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

    public override StaticCreatureCardEffectBase GetStaticCreatureEffectBase() {
        return effectBase;
    }

    public override CreatureCardEffectPayload GetEffectPayload() {
        return new OverwhelmEffectPayload(this);
    }
}
