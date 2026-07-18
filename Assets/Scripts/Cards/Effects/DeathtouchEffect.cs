using System;

[Serializable]
public class DeathtouchEffect : StaticCreatureCardEffect {
    private static readonly string EFFECT_NAME = "Deathtouch";
    private static readonly string EFFECT_DESCRIPTION = "When this creature deals damage to another creature, that creature dies.";

    public DeathtouchEffect() : base() {
        effectName = EFFECT_NAME;
        description = EFFECT_DESCRIPTION;
        effectIconId = "";
    }

    public DeathtouchEffect(DeathtouchEffect effect) : this() { }

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

    public override string GetFullDescription() {
        return description;
    }

    public override CreatureCardEffect DeepCopy() {
        return new DeathtouchEffect(this);
    }

    public override CreatureCardEffectPayload GetEffectPayload() {
        return new DeathtouchEffectPayload(this);
    }
}