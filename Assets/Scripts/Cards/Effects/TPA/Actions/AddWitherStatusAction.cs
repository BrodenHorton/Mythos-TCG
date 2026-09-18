public class AddWitherStatusAction : EffectAction<EffectContext<CreatureCard>, CreatureCombatDamageEventArgs, CreatureCard> {
    public void Execute(EffectContext<CreatureCard> context, CreatureCombatDamageEventArgs args) {
        TcgLogger.Log("Wither Effect triggered");
        int damage = args.Damage;
        CreatureCombatDamageEventArgs witherArgs = new CreatureCombatDamageEventArgs(args.InitiatorId,
                                                                                     args.TargetId,
                                                                                     args.Attacker,
                                                                                     args.Defender,
                                                                                     damage);
        EventBus.Instance.InvokeOnWitherProked(witherArgs);
        if (!witherArgs.IsCanceled) {
            TcgLogger.Log("Wither Status added to Defender");
            // TODO: Replace with get method to the CardEffectRegistry to get WithStatusEffect
            args.Defender.AddEffect(new WitherStatusEffect(effectBase.WitherStatusEffectBase, damage));
        }
    }
}