
public abstract class DynamicCreatureCardEffect : CreatureCardEffect {

    public override string GetFullDescription() {
        return "<color=#fafa64>" + GetDynamicCreatureEffectBase().DynamicEffectType + "</color>" + ": " + GetDynamicCreatureEffectBase().Description;
    }

    public abstract DynamicCreatureCardEffectBase GetDynamicCreatureEffectBase();

    public override CreatureCardEffectBase GetCreatureEffectBase() {
        return GetDynamicCreatureEffectBase();
    }
}