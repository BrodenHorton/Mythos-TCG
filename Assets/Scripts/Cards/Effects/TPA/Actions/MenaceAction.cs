public class MenaceAction : EffectAction<MenaceContext, CanDefendEventArgs, CreatureCard> {
    public void Execute(MenaceContext context, CanDefendEventArgs args) {
        if (args.Defender.GetHealth() < context.BlockableHealthMin)
            args.CanDefend = false;
    }
}