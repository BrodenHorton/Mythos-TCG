
public class SwarmAddEffectContext : EffectContext<CreatureCard> {
    private CreatureCardEffectType additionalEffectType;
    private CardEffect<CreatureCard> addedEffect;

    public SwarmAddEffectContext(CreatureCardEffectType additionalEffectType) {
        this.additionalEffectType = additionalEffectType;
        addedEffect = null;
    }

    public CreatureCardEffectType AdditionalEffectType { get { return additionalEffectType; } }

    public CardEffect<CreatureCard> AddedEffect { get { return addedEffect; } set { addedEffect = value; } }
}