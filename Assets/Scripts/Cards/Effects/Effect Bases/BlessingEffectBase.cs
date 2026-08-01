using System;

public abstract class BlessingEffectBase : DynamicCreatureCardEffectBase {
    
    public override string GetDynamicEffectType() {
        return "Blessing";
    }
}