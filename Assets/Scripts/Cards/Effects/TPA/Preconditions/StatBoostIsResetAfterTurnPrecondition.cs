using System;

public class StatBoostIsResetAfterTurnPrecondition : EffectPrecondition<StatBoostEffectContext, EventArgs> {
    public bool Evaluate(StatBoostEffectContext context, EventArgs _) {
        return context.IsResetAfterTurn;
    }
}