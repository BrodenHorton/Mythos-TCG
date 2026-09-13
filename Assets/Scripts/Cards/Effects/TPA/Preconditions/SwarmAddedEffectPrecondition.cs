using System;

public class SwarmAddedEffectPrecondition : EffectPrecondition<SwarmAddEffectContext, EventArgs> {
    public bool Evaluate(SwarmAddEffectContext context, EventArgs _) {
        return context.AddedEffect == null;
    }
}