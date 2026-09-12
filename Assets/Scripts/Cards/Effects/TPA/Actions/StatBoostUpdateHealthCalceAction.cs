public class StatBoostUpdateHealthCalceAction : EffectAction<StatBoostEffectContext, PlayerCardStatEventArgs<CreatureCard>> {
    public void Execute(StatBoostEffectContext context, PlayerCardStatEventArgs<CreatureCard> args) {
        args.Value += context.EffectProkCount * context.HealthBoost;
    }
}
