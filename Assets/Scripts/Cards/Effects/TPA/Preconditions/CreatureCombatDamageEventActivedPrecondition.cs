public class CreatureCombatDamageEventActivedPrecondition : EffectPrecondition<EffectContext<CreatureCard>, CreatureCombatDamageEventArgs> {
    public bool Evaluate(EffectContext<CreatureCard> context, CreatureCombatDamageEventArgs args) {
        return !args.IsCanceled;
    }
}