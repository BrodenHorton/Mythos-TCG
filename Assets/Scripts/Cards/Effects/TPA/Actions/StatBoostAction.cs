using System;

public class StatBoostIncrementAction : EffectAction<StatBoostEffectContext, EventArgs> {
    public void Execute(StatBoostEffectContext context, EventArgs _) {
        context.EffectProkCount++;
    }
}
