public abstract class SummonEffect : CreatureCardEffect {
    private static readonly string SUMMON_KEYWORD = "summon";

    protected abstract void SummonEffectHandler(object sender, PlayerCardEventArgs<CreatureCard> args);

    public abstract string GetDynamicEffectDescription();

    public sealed override string GetRawDescription() {
        return CardRichTextUtil.GetKeywordLinkTagText(SUMMON_KEYWORD, "Summon") + ": " + GetDynamicEffectDescription();
    }
}
