public class SwarmAddEffectAction : EffectAction<SwarmAddEffectContext, PlayerCardEventArgs<CreatureCard>, CreatureCard> {
    public void Execute(SwarmAddEffectContext context, PlayerCardEventArgs<CreatureCard> args) {
        context.AddedEffect = CardEffectRegistry.Get(context.AdditionalEffectType);
        args.Card.AddEffect(context.AddedEffect);
    }
}
