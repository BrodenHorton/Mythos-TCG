public class DamageDealtPrecondition : EffectPrecondition<EffectContext<CreatureCard>, CreatureCombatDamageEventArgs, CreatureCard> {
    public bool Evaluate(EffectContext<CreatureCard> context, CreatureCombatDamageEventArgs args) {
        return args.Damage > 0;
    }
}