public class WitherStatustCalcUpdateAction : EffectAction<WitherStatusContext, PlayerCardStatEventArgs<CreatureCard>, CreatureCard> {
    public void Execute(WitherStatusContext context, PlayerCardStatEventArgs<CreatureCard> args) {
        args.Value -= context.WitherProkCount;
    }
}