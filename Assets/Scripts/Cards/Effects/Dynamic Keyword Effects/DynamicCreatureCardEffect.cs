
public abstract class DynamicCreatureCardEffect : CreatureCardEffect {

    public abstract string GetEffectDescription();

    public sealed override string GetFullDescription() {
        return "<color=#fff47d>" + GetDynamicKeyword().KeywordName + "</color>: " + GetEffectDescription();
    }

    public abstract DynamicKeyword GetDynamicKeyword();
}