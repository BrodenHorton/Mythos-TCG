using System;

[Serializable]
public class WitherEffect : CreatureCardEffect {

    public WitherEffect() : base() { }

    public WitherEffect(WitherEffect effect) : base() { }

    public override void Init(Guid creatureCardUuid) {
        this.creatureCardUuid = creatureCardUuid;
        EventBus.Instance.OnCreatureDamagedByCreature += AddWitherStatus;
    }

    public override void RemoveListeners() {
        EventBus.Instance.OnCreatureDamagedByCreature -= AddWitherStatus;
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
                                                                                     damage);
        EventBus.Instance.InvokeOnWitherProked(witherArgs);
        args.ShouldDamageDefender = false;
        if(!witherArgs.IsCanceled) {
            TcgLogger.Log("Wither Status added to Defender");
            args.Defender.AddEffect(new WitherStatusEffect(damage));
        }
    }

    public override CreatureCardEffect DeepCopy() {
        return new WitherEffect(this);
    }

    public override CreatureCardEffectPayload GetEffectPayload() {
        return new WitherEffectPayload(this);
    }
}
