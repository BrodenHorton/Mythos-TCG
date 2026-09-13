public class SwarmRemoveEffectAction : EffectAction<SwarmAddEffectContext, PlayerCardEventArgs<CreatureCard>> {
    public void Execute(SwarmAddEffectContext context, PlayerCardEventArgs<CreatureCard> args) {
        args.Card.RemoveEffect(context.AddedEffect);
        context.AddedEffect = null;
    }
}