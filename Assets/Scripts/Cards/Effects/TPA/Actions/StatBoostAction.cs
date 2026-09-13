using System;

public class StatBoostIncrementAction : EffectAction<StatBoostContext, EventArgs> {
    public void Execute(StatBoostContext context, EventArgs _) {
        context.EffectProkCount++;
    }
}
