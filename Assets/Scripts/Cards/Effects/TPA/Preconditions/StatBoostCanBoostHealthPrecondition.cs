using System;

public class StatBoostCanBoostHealthPrecondition : EffectPrecondition<StatBoostContext, EventArgs> {
    public bool Evaluate(StatBoostContext context, EventArgs _) {
        return context.EffectProkCount > 0 && context.HealthBoost > 0;
    }
}
