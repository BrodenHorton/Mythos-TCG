
public class LifeGainContext : EffectContext<CreatureCard> {
    private int lifePointsModifier;

    public LifeGainContext(string id, string effectName, int lifePointsModifier) : base(id, effectName) {
        this.lifePointsModifier = lifePointsModifier;
    }

    public int LifePointsModifier { get { return lifePointsModifier; } }
}
