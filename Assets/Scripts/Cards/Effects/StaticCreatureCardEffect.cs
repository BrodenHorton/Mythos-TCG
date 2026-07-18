public abstract class StaticCreatureCardEffect : CreatureCardEffect {
    protected string effectIconId;

    protected StaticCreatureCardEffect() { }

    public string EffectIconId { get { return effectIconId; } }
}