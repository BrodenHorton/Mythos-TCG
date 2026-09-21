using System;

public class DuelistDefenderExistsPrecondition : EffectPrecondition<DuelistContext, EventArgs, CreatureCard> {
    public bool Evaluate(DuelistContext context, EventArgs args) {
        return context.DuelistDefender != null;
    }
}
