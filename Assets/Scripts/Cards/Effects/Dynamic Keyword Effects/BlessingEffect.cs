public abstract class BlessingEffect : CreatureCardEffect {
    private static readonly string BLESSING_KEYWORD = "blessing";

    protected abstract void BlessingEffectHandler(object sender, LifePointsChangedEventArgs args);

    public abstract string GetDynamicEffectDescription();

    public sealed override string GetRawDescription() {
        return CardRichTextUtil.GetKeywordLinkTagText(BLESSING_KEYWORD, "Blessing") + ": " + GetDynamicEffectDescription();
    }
}
