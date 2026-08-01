
public abstract class StaticCreatureCardEffectPayload : CreatureCardEffectPayload {
    protected string iconId;

    public StaticCreatureCardEffectPayload() { }

    public StaticCreatureCardEffectPayload(StaticCreatureCardEffect effect) : base(effect) {
        iconId = effect.GetStaticCreatureEffectBase().EffectIconId;
    }

    public string IconId { get { return iconId; } }
}