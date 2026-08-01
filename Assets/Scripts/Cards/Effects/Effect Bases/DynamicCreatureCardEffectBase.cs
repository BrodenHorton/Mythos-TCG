using System;

public abstract class DynamicCreatureCardEffectBase : CreatureCardEffectBase {

    public abstract string GetDynamicEffectType();

    public abstract string GetDynamicDescription();

    public override string GetFullDescription() {
        return "<color=#fff47d>" + GetDynamicEffectType() + "</color>: " + GetDynamicDescription();
    }
}
