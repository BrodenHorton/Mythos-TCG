using System;

public class ClearDuelistDefenderAction : EffectAction<DuelistContext, EventArgs, CreatureCard> {
    public void Execute(DuelistContext context, EventArgs args) {
        context.DuelistDefender = null;
    }
}