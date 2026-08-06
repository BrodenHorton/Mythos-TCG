public abstract class SummonEffect : DynamicCreatureCardEffect {

    protected abstract void SummonEffectHandler(object sender, PlayerCardEventArgs<CreatureCard> args);

    public sealed override DynamicKeyword GetDynamicKeyword() {
        return ServiceLocator.Get<DynamicKeywordRegistry>().Get("summon");
    }
}
