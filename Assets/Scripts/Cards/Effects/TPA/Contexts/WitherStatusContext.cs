public class WitherStatusContext : EffectContext<CreatureCard> {
    private int witherProkCount;

    public WitherStatusContext(string id, string effectName) : base(id, effectName) {
        witherProkCount = 0;
    }

    public int WitherProkCount { get { return witherProkCount; } set { witherProkCount = value; } }
}