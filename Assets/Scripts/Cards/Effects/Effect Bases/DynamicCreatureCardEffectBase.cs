
public abstract class DynamicCreatureCardEffectBase : CreatureCardEffectBase {

    public abstract string GetEffectDescription();

    public sealed override string GetFullDescription() {
        return "<color=#fff47d>" + GetDynamicKeyword().KeywordName + "</color>: " + GetEffectDescription();
    }

    public abstract DynamicKeyword GetDynamicKeyword();
}
