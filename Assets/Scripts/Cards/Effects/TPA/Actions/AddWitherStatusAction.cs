public class AddWitherStatusAction : EffectAction<EffectContext<CreatureCard>, CreatureCombatDamageEventArgs, CreatureCard> {
    public void Execute(EffectContext<CreatureCard> context, CreatureCombatDamageEventArgs args) {
        TcgLogger.Log("Wither Effect triggered");
        int damage = args.Damage;
        CreatureCombatDamageEventArgs witherArgs = new CreatureCombatDamageEventArgs(args.InitiatorId,
                                                                                     args.TargetId,
                                                                                     args.Attacker,
                                                                                     args.Defender,
                                                                                     damage);
        EventBus.Instance.InvokeOnWitherProked(witherArgs); // TODO: Create a new event for checking if there is an existing Wither status on a creature
        if (!witherArgs.IsCanceled) {
            TcgLogger.Log("Wither Status added to Defender");
            args.Defender.AddEffect(CardEffectRegistry.Get(CreatureCardEffectType.WitherStatus));
            EventBus.Instance.InvokeOnWitherProked(witherArgs);
        }
    }
}
