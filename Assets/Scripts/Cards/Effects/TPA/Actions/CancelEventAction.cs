public class CancelEventAction : EffectAction<EffectContext<CreatureCard>, PlayerCardCancelableEventArgs<CreatureCard>, CreatureCard> {
    public void Execute(EffectContext<CreatureCard> context, PlayerCardCancelableEventArgs<CreatureCard> args) {
        args.IsCanceled = true;
    }
}
