public class PlayerCheckPrecondition : EffectPrecondition<EffectContext<CreatureCard>, PlayerEventArgs, CreatureCard> {
    public bool Evaluate(EffectContext<CreatureCard> context, PlayerEventArgs args) {
        return context.Card.PlayerId == args.PlayerId;
    }
}