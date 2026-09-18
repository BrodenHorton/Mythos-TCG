
public class MenaceContext : EffectContext<CreatureCard> {
    private int blockableHealthMin;

    public MenaceContext(string id, string effectName) : base(id, effectName) {
        blockableHealthMin = 4;
    }

    public int BlockableHealthMin { get { return blockableHealthMin; } }
}