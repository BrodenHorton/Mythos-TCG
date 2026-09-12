using System;

public class StatBoostCanBoostHealthPrecondition : EffectPrecondition<StatBoostEffectContext, EventArgs> {
    public bool Evaluate(StatBoostEffectContext context, EventArgs _) {
        return context.EffectProkCount > 0 && context.HealthBoost > 0;
    }
}
