
public class CardSearchContext<TCard> : EffectContext<TCard> where TCard : Card {
    private CardBase searchTarget;

    public CardSearchContext(string id, string effectName, CardBase searchTarget) : base(id, effectName) {
        this.searchTarget = searchTarget;
    }

    public CardBase SearchTarget { get { return searchTarget; } }
}
