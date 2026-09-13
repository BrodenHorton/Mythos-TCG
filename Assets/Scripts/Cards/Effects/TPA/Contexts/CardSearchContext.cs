
public class CardSearchContext : EffectContext<CreatureCard> {
    private CardBase searchTarget;

    public CardSearchContext(string id, string effectName, CardBase searchTarget) : base(id, effectName) {
        this.searchTarget = searchTarget;
    }

    public CardBase SearchTarget { get { return searchTarget; } }
}
