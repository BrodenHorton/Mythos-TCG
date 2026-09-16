public class DefenderExistsPrecondition : EffectPrecondition<EffectContext<CreatureCard>, CreatureCombatDamageEventArgs, CreatureCard> {
    public bool Evaluate(EffectContext<CreatureCard> context, CreatureCombatDamageEventArgs args) {
        return args.Defender != null;
    }
}