using System;

public class StatBoostCanBoostHealthPrecondition : EffectPrecondition<StatBoostContext, EventArgs, CreatureCard> {
    public bool Evaluate(StatBoostContext context, EventArgs _) {
        return context.EffectProkCount > 0 && context.HealthBoost > 0;
    }
}
