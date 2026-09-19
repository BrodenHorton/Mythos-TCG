using System;

public class WitherStatusHasProksPrecondition : EffectPrecondition<WitherStatusContext, EventArgs, CreatureCard> {
    public bool Evaluate(WitherStatusContext context, EventArgs _) {
        return context.WitherProkCount > 0;
    }
}