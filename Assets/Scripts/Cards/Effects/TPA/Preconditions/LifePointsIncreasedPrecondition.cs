
#endregion

#region Preconditions
public class LifePointsIncreasedPrecondition : EffectPrecondition<EffectContext<CreatureCard>, LifePointsChangedEventArgs> {
    public bool Evaluate(EffectContext<CreatureCard> context, LifePointsChangedEventArgs args) {
        return args.LifePoints > args.PreviousLifePoints;
    }
}
#endregion
