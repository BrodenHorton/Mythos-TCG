public abstract class EffectContext<TCard> where TCard : Card {
    private string id;
    private string effectName;
    private TCard card;

    public EffectContext(string id, string effectName) {
        this.id = id;
        this.effectName = effectName;
    }

    public abstract void Init(TCard card);

    public abstract void RemoveListeners();

    public string Id { get { return id; } }

    public string EffectName { get { return effectName; } }

    public TCard Card { get { return card; } }
}
#endregion
