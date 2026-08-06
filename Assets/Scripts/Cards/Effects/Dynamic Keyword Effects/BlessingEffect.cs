public abstract class BlessingEffect : DynamicCreatureCardEffect {

    protected abstract void BlessingEffectHandler(object sender, LifePointsChangedEventArgs args);

    public sealed override DynamicKeyword GetDynamicKeyword() {
        return ServiceLocator.Get<DynamicKeywordRegistry>().Get("blessing");
    }
}
