
public class SwarmAddEffectContext : EffectContext<CreatureCard> {
    private CreatureCardEffectType additionalEffectType;
    private CreatureCardEffect addedEffect;

    public SwarmAddEffectContext(string id, string effectName, CreatureCardEffectType additionalEffectType) : base(id, effectName) {
        this.additionalEffectType = additionalEffectType;
        addedEffect = null;
    }

    public CreatureCardEffectType AdditionalEffectType { get { return additionalEffectType; } }

    public CreatureCardEffect AddedEffect { get { return addedEffect; } set { addedEffect = value; } }
}