using System;

public class StatBoostClearEffectProksAction : EffectAction<StatBoostEffectContext, EventArgs> {
    public void Execute(StatBoostEffectContext context, EventArgs _) {
        context.EffectProkCount = 0;
    }
}