using System;

[Serializable]
public class WitherEffect : CreatureCardEffect {

    public WitherEffect() : base() { }

    public WitherEffect(WitherEffect effect) : base() { }

    public override void Init(Guid creatureCardUuid) {
        this.creatureCardUuid = creatureCardUuid;
        EventBus.Instance.OnWitherProked += AddWitherStatus;
    }

    public override void RemoveListeners() {
        EventBus.Instance.OnWitherProked -= AddWitherStatus;
    }

    private void AddWitherStatus(object sender, CreatureCombatDamageEventArgs args) {
        if (args.Attacker.Uuid != creatureCardUuid)
            return;
        if (args.IsCanceled)
            return;
        if (args.Defender == null)
            return;

        TcgLogger.Log("Wither Effect triggered");
        int damage = args.Damage;
        CreatureCombatDamageEventArgs witherArgs = new CreatureCombatDamageEventArgs(args.InitiatorId,
                                                                                     args.TargetId,
                                                                                     args.Attacker,
                                                                                     args.Defender,
                                                                                     ref damage);
        EventBus.Instance.InvokeOnWitherProked(witherArgs);
        if(!witherArgs.IsCanceled) {
            args.Damage = 0;
            args.Defender.Effects.Add(new WitherStatusEffect(damage));
        }
    }

    public override CreatureCardEffect DeepCopy() {
        return new WitherEffect(this);
    }

    public override CreatureCardEffectPayload GetEffectPayload() {
        return new WitherEffectPayload(this);
    }
}
