public class StatBoostUpdateAttackCalceAction : EffectAction<StatBoostContext, PlayerCardStatEventArgs<CreatureCard>> {
    public void Execute(StatBoostContext context, PlayerCardStatEventArgs<CreatureCard> args) {
        args.Value += context.EffectProkCount * context.AtkBoost;
    }
}
