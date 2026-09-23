public class WitherStatusContext : EffectContext<CreatureCard> {
    private int witherProkCount;

    public WitherStatusContext() {
        witherProkCount = 0;
    }

    public int WitherProkCount { get { return witherProkCount; } set { witherProkCount = value; } }
}