public abstract class BlessingEffect : CreatureCardEffect {

    public BlessingEffect(BlessingEffectBase effectBase) { }

    protected abstract void BlessingEffectHandler(object sender, LifePointsChangedEventArgs args);
}