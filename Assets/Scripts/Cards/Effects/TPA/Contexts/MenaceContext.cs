
public class MenaceContext : EffectContext<CreatureCard> {
    private int blockableHealthMin;

    public MenaceContext() {
        blockableHealthMin = 4;
    }

    public int BlockableHealthMin { get { return blockableHealthMin; } }
}