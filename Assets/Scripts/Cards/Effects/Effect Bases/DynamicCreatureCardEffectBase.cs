
public abstract class DynamicCreatureCardEffectBase : CreatureCardEffectBase {

    public abstract string GetEffectDescription();

    public override string GetFullDescription() {
        return "<color=#fff47d>" + GetDynamicKeyword().KeywordName + "</color>: " + GetEffectDescription();
    }

    public abstract DynamicKeyword GetDynamicKeyword();
}

public abstract class BlessingEffectBase : DynamicCreatureCardEffectBase {

    public override DynamicKeyword GetDynamicKeyword() {
        return ServiceLocator.Get<DynamicKeywordRegistry>().Get("blessing");
    }
}