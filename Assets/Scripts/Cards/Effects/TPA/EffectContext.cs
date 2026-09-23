public class EffectContext<TCard> where TCard : Card {
    protected TCard card;

    public EffectContext() { }

    public TCard Card { get { return card; } }
}
