
public class StatBoostContext : EffectContext<CreatureCard> {
    private int atkBoost;
    private int healthBoost;
    private bool isResetAfterTurn;
    private int effectProkCount;

    public StatBoostContext(string id, string effectName, int atkBoost, int healthBoost, bool isResetAfterTurn) : base(id, effectName) {
        this.atkBoost = atkBoost;
        this.healthBoost = healthBoost;
        this.isResetAfterTurn = isResetAfterTurn;
        effectProkCount = 0;
    }

    public int AtkBoost { get {  return atkBoost; } }

    public int HealthBoost { get { return healthBoost; } }

    public bool IsResetAfterTurn {  get { return isResetAfterTurn; } }

    public int EffectProkCount { get { return effectProkCount; } set { effectProkCount = value; } }
}
