using System;

public class StatBoostCanBoostAttackPrecondition : EffectPrecondition<StatBoostEffectContext, EventArgs> {
    public bool Evaluate(StatBoostEffectContext context, EventArgs _) {
        return context.EffectProkCount > 0 && context.AtkBoost > 0;
    }
}
