
public class DuelistContext : EffectContext<CreatureCard> {
    private CreatureCard duelistDefender;

    public DuelistContext() {
        duelistDefender = null;
    }

    public CreatureCard DuelistDefender { get { return duelistDefender; } set { duelistDefender = value; } }
}