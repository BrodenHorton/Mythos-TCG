using System;

public class StatBoostAction : EffectAction<StatBoostEffectContext, EventArgs> {
    public void Execute(StatBoostEffectContext context, EventArgs _) {
        context.EffectProkCount++;
    }
}
