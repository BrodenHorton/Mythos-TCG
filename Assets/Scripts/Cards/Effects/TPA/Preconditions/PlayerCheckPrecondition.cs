public class PlayerCheckPrecondition<TCard> : EffectPrecondition<EffectContext<TCard>, PlayerEventArgs, TCard> where TCard : Card {
    public bool Evaluate(EffectContext<TCard> context, PlayerEventArgs args) {
        return context.Card.PlayerId == args.PlayerId;
    }
}
