public abstract class SummonEffect : DynamicCreatureCardEffect {

    protected abstract void SummonEffectHandler(object sender, PlayerCardEventArgs<CreatureCard> args);

    public sealed override EffectKeyword GetDynamicKeyword() {
        return ServiceLocator.Get<DynamicKeywordRegistry>().Get("summon");
    }
}
