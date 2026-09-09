
public class CardSearchEffectContext : EffectContext<CreatureCard> {
    private CardBase searchTarget;

    public CardSearchEffectContext(string id, string effectName, CardBase searchTarget) : base(id, effectName) {
        this.searchTarget = searchTarget;
    }

    public override void Init(CreatureCard card) { }

    public override void RemoveListeners() { }

    public CardBase SearchTarget { get { return searchTarget; } }
}