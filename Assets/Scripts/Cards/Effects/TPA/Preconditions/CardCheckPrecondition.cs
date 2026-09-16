
public class CardCheckPrecondition<TCard> : EffectPrecondition<EffectContext<TCard>, PlayerCardEventArgs<TCard>, TCard> where TCard : Card {
    public bool Evaluate(EffectContext<TCard> context, PlayerCardEventArgs<TCard> args) {
        return context.Card.Uuid == args.Card.Uuid;
    }
}