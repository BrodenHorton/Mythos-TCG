public class StatBoostUpdateAttackCalcAction : EffectAction<StatBoostContext, PlayerCardStatEventArgs<CreatureCard>, CreatureCard> {
    public void Execute(StatBoostContext context, PlayerCardStatEventArgs<CreatureCard> args) {
        args.Value += context.EffectProkCount * context.AtkBoost;
    }
}
