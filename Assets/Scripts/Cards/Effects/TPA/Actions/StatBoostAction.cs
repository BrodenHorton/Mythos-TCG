using System;

public class StatBoostIncrementAction : EffectAction<StatBoostContext, EventArgs, CreatureCard> {
    public void Execute(StatBoostContext context, EventArgs _) {
        context.EffectProkCount++;
    }
}
