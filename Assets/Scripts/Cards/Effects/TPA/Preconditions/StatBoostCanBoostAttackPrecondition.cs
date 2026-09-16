using System;

public class StatBoostCanBoostAttackPrecondition : EffectPrecondition<StatBoostContext, EventArgs, CreatureCard> {
    public bool Evaluate(StatBoostContext context, EventArgs _) {
        return context.EffectProkCount > 0 && context.AtkBoost > 0;
    }
}
