using System;

[Serializable]
public class DeathtouchEffect : StaticCreatureCardEffect {
    private DeathtouchEffectBase effectBase;

    public DeathtouchEffect(DeathtouchEffectBase effectBase) {
        this.effectBase = effectBase;
    }

    public override void Init(CreatureCard card) {
        this.card = card;
        EventBus.Instance.OnCreatureCombatFinished += DestroyDefender;
    }

    public override void RemoveListeners() {
        EventBus.Instance.OnCreatureCombatFinished -= DestroyDefender;
    }

    private void DestroyDefender(object sender, CreatureCombatDamageEventArgs args) {
        if (args.Attacker.Uuid != card.Uuid)
            return;
        if (args.Defender == null)
            return;

        TcgLogger.Log("Deathtouch Effect triggered");
        args.Defender.DestroyCreature();
    }

    public override StaticCreatureCardEffectBase GetStaticCreatureEffectBase() {
        return effectBase;
    }

    public override CreatureCardEffectPayload GetEffectPayload() {
        return new DeathtouchEffectPayload(this);
    }
}