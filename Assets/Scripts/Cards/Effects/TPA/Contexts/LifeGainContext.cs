public class LifeGainContext : EffectContext<CreatureCard> {
    private int lifePointsModifier;

    public LifeGainContext(int lifePointsModifier) {
        this.lifePointsModifier = lifePointsModifier;
    }

    public int LifePointsModifier { get { return lifePointsModifier; } }
}
