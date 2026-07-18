using System;

[Serializable]
public class WitherEffect : StaticCreatureCardEffect {
    private static readonly string EFFECT_NAME = "Wither";
    private static readonly string EFFECT_DESCRIPTION = "Deals damage as -1/-1 debuffs.";

    public WitherEffect() : base() {
        effectName = EFFECT_NAME;
        description = EFFECT_DESCRIPTION;
        effectIconId = "";
    }

    public WitherEffect(WitherEffect effect) : this() { }

    public override void Init(CreatureCard card) {
        this.card = card;
        EventBus.Instance.OnCreatureDamagedByCreature += StopDamageToDefender;
        EventBus.Instance.OnCreatureDamagedByCreatureFinished += AddWitherStatus;
    }

    public override void RemoveListeners() {
        EventBus.Instance.OnCreatureDamagedByCreature -= StopDamageToDefender;
        EventBus.Instance.OnCreatureDamagedByCreatureFinished -= AddWitherStatus;
    }

    private void StopDamageToDefender(object sender, CreatureCombatDamageEventArgs args) {
        if (args.Attacker.Uuid != card.Uuid)
            return;
        if (args.IsCanceled)
            return;
        if (args.Defender == null)
            return;

        args.ShouldDamageDefender = false;
    }

    private void AddWitherStatus(object sender, CreatureCombatDamageEventArgs args) {
        if (args.Attacker.Uuid != card.Uuid)
            return;
        if (args.Defender == null)
            return;

        TcgLogger.Log("Wither Effect triggered");
        int damage = args.Damage;
        CreatureCombatDamageEventArgs witherArgs = new CreatureCombatDamageEventArgs(args.InitiatorId,
                                                                                     args.TargetId,
                                                                                     args.Attacker,
                                                                                     args.Defender,
                                                                                     damage);
        EventBus.Instance.InvokeOnWitherProked(witherArgs);
        if(!witherArgs.IsCanceled) {
            TcgLogger.Log("Wither Status added to Defender");
            args.Defender.AddEffect(new WitherStatusEffect(damage));
        }
    }

    public override string GetFullDescription() {
        return description;
    }

    public override CreatureCardEffect DeepCopy() {
        return new WitherEffect(this);
    }

    public override CreatureCardEffectPayload GetEffectPayload() {
        return new WitherEffectPayload(this);
    }
}
