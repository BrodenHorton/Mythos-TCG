using System;

public class StatBoostCanBoostAttackPrecondition : EffectPrecondition<StatBoostContext, EventArgs> {
    public bool Evaluate(StatBoostContext context, EventArgs _) {
        return context.EffectProkCount > 0 && context.AtkBoost > 0;
    }
}
