public abstract class SwarmEffect : CreatureCardEffect {
    private static readonly string SWARM_KEYWORD = "swarm";
    protected static readonly int SWARM_MINIMUM = 4;

    protected abstract void SwarmEffectHandler(object sender, PlayerCardEventArgs<CreatureCard> args);

    protected abstract void ClearSwarmEffect(object sender, PlayerCardEventArgs<CreatureCard> args);

    public abstract string GetDynamicEffectDescription();

    public sealed override string GetRawDescription() {
        return CardRichTextUtil.GetKeywordLinkTagText(SWARM_KEYWORD, "Swarm") + ": " + GetDynamicEffectDescription();
    }
}