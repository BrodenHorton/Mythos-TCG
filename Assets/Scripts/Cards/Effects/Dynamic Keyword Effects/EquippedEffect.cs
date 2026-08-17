public abstract class EquippedEffect : CreatureCardEffect {
    private static readonly string EQUIPPED_KEYWORD = "equipped";

    // TODO: Change to listener for when a creature is the target of a spell
    protected abstract void EquippedEffectHandler(object sender, ulong currentPlayerTurnId);

    public abstract string GetDynamicEffectDescription();

    public sealed override string GetRawDescription() {
        return CardRichTextUtil.GetKeywordLinkTagText(EQUIPPED_KEYWORD, "Equipped") + ": " + GetDynamicEffectDescription();
    }
}
