
public abstract class StaticCreatureCardEffectPayload : CreatureCardEffectPayload {
    protected string iconId;

    public StaticCreatureCardEffectPayload() { }

    public StaticCreatureCardEffectPayload(StaticCreatureCardEffect effect) {
        effectName = effect.EffectName;
        description = effect.GetFullDescription();
        creatureUuidStr = effect.Card.Uuid.ToString();
        effectType = CreatureCardEffectType.Bloodthirsty;
        iconId = effect.EffectIconId;
    }

    public string IconId { get { return iconId; } }
}