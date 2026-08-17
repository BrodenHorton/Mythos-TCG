public abstract class BattleCryEffect : CreatureCardEffect {
    private static readonly string BATTLE_CRY_KEYWORD = "battle_cry";

    protected abstract void BattleCryEffectHandler(object sender, ulong currentPlayerTurnId);

    public abstract string GetDynamicEffectDescription();

    public sealed override string GetRawDescription() {
        return CardRichTextUtil.GetKeywordLinkTagText(BATTLE_CRY_KEYWORD, "Battle Cry") + ": " + GetDynamicEffectDescription();
    }
}
