public abstract class DeathCryEffect : CreatureCardEffect {
    private static readonly string DEATH_CRY_KEYWORD = "death_cry";

    protected abstract void DeathCryEffectHandler(object sender, PlayerCardEventArgs<CreatureCard> args);

    public abstract string GetDynamicEffectDescription();

    public sealed override string GetRawDescription() {
        return CardRichTextUtil.GetKeywordLinkTagText(DEATH_CRY_KEYWORD, "Death Cry") + ": " + GetDynamicEffectDescription();
    }
}