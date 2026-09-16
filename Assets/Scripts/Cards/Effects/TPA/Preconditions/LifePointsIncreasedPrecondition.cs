public class LifePointsIncreasedPrecondition : EffectPrecondition<EffectContext<CreatureCard>, LifePointsChangedEventArgs, CreatureCard> {
    public bool Evaluate(EffectContext<CreatureCard> context, LifePointsChangedEventArgs args) {
        return args.LifePoints > args.PreviousLifePoints;
    }
}
