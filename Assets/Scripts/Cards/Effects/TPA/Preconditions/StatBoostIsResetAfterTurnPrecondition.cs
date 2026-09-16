using System;

public class StatBoostIsResetAfterTurnPrecondition : EffectPrecondition<StatBoostContext, EventArgs, CreatureCard> {
    public bool Evaluate(StatBoostContext context, EventArgs _) {
        return context.IsResetAfterTurn;
    }
}