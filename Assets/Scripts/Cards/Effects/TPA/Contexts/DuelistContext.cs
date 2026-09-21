
public class DuelistContext : EffectContext<CreatureCard> {
    private CreatureCard duelistDefender;

    public DuelistContext(string id, string effectName) : base(id, effectName) {
        duelistDefender = null;
    }

    public CreatureCard DuelistDefender { get { return duelistDefender; } set { duelistDefender = value; } }
}