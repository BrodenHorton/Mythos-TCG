public class EffectContext<TCard> where TCard : Card {
    private string id;
    private string effectName;
    protected TCard card;

    public EffectContext(string id, string effectName) {
        this.id = id;
        this.effectName = effectName;
    }

    public string Id { get { return id; } }

    public string EffectName { get { return effectName; } }

    public TCard Card { get { return card; } }
}
