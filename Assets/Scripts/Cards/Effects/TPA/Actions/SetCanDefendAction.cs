public class SetCanDefendAction : EffectAction<EffectContext<CreatureCard>, CanDefendEventArgs, CreatureCard> {
    public void Execute(EffectContext<CreatureCard> context, CanDefendEventArgs args) {
        args.CanDefend = true;
    }
}