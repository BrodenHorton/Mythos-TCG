
public abstract class DynamicCreatureCardEffect : CreatureCardEffect {

    public abstract DynamicCreatureCardEffectBase GetDynamicCreatureEffectBase();

    public override CreatureCardEffectBase GetCreatureEffectBase() {
        return GetDynamicCreatureEffectBase();
    }
}