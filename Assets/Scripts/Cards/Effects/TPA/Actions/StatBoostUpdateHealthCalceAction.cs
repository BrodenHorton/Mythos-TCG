public class StatBoostUpdateHealthCalceAction : EffectAction<StatBoostContext, PlayerCardStatEventArgs<CreatureCard>, CreatureCard> {
    public void Execute(StatBoostContext context, PlayerCardStatEventArgs<CreatureCard> args) {
        args.Value += context.EffectProkCount * context.HealthBoost;
    }
}
