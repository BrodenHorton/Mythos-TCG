public class WitherContext : EffectContext<CreatureCard> {
    private int effectProkCount;

    public WitherContext(string id, string effectName) : base(id, effectName) {
        effectProkCount = 0;
    }

    public int EffectProkCount { get { return effectProkCount; } set { effectProkCount = value; } }
}