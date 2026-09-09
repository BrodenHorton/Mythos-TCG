
public class CardCheckPrecondition : EffectPrecondition<EffectContext<CreatureCard>, PlayerCardEventArgs<CreatureCard>> {
    public bool Evaluate(EffectContext<CreatureCard> context, PlayerCardEventArgs<CreatureCard> args) {
        return context.Card.Uuid == args.Card.Uuid;
    }
}