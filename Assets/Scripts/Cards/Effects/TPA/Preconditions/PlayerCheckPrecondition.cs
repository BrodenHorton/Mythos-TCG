public class PlayerCheckPrecondition : EffectPrecondition<EffectContext<CreatureCard>, PlayerEventArgs> {
    public bool Evaluate(EffectContext<CreatureCard> context, PlayerEventArgs args) {
        return context.Card.PlayerId == args.PlayerId;
    }
}
