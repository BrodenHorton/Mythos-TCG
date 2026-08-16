
public abstract class DynamicCreatureCardEffect : CreatureCardEffect {

    public abstract string GetDynamicEffectDescription();

    public sealed override string GetRawDescription() {
        return "<color=#fff47d><link=keyword_id>" + GetDynamicKeyword().Id + "</link></color>: " + GetDynamicEffectDescription();
    }

    public abstract EffectKeyword GetDynamicKeyword();
}