public abstract class StaticCreatureCardEffect : CreatureCardEffect {

    public abstract StaticCreatureCardEffectBase GetStaticCreatureEffectBase();

    public override CreatureCardEffectBase GetCreatureEffectBase() {
        return GetStaticCreatureEffectBase();
    }
}