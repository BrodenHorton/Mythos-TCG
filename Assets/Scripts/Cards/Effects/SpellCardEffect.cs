[System.Serializable]
public abstract class SpellCardEffect {
    protected SpellCard card;

    public SpellCardEffect() {}

    public void Init(SpellCard card) {
        this.card = card;
    }

    public abstract void AddListener();

    public abstract void RemoveListener();

    public abstract void Execute();
}