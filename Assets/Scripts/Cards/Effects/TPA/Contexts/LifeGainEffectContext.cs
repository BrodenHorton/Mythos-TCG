
public class LifeGainEffectContext : EffectContext<CreatureCard> {
    private int lifePointsModifier;

    public LifeGainEffectContext(string id, string effectName, int lifePointsModifier) : base(id, effectName) {
        this.lifePointsModifier = lifePointsModifier;
    }

    public int LifePointsModifier { get { return lifePointsModifier; } }
}