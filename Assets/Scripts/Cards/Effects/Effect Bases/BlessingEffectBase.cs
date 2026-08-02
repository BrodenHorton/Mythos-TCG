
public abstract class BlessingEffectBase : DynamicCreatureCardEffectBase {

    public abstract BlessingEffect CreateBlessingCreatureCardEffect();

    public sealed override CreatureCardEffect CreateCreatureCardEffect() {
        return CreateBlessingCreatureCardEffect();
    }

    public sealed override DynamicKeyword GetDynamicKeyword() {
        return ServiceLocator.Get<DynamicKeywordRegistry>().Get("blessing");
    }
}