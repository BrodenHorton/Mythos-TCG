using System;

public class StatBoostClearEffectProksAction : EffectAction<StatBoostContext, EventArgs, CreatureCard> {
    public void Execute(StatBoostContext context, EventArgs _) {
        context.EffectProkCount = 0;
    }
}