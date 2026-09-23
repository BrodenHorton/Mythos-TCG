
public class CardSearchContext<TCard> : EffectContext<TCard> where TCard : Card {
    private CardBase searchTarget;

    public CardSearchContext(CardBase searchTarget) {
        this.searchTarget = searchTarget;
    }

    public CardBase SearchTarget { get { return searchTarget; } }
}
