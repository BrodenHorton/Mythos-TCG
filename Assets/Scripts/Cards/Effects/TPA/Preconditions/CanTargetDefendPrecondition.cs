public class CanTargetDefendPrecondition : EffectPrecondition<EffectContext<CreatureCard>, CanDefendEventArgs, CreatureCard> {
    public bool Evaluate(EffectContext<CreatureCard> context, CanDefendEventArgs args) {
        return args.CanDefend;
    }
}