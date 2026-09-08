using System;

public interface EffectPrecondition<in T, in U> where T : EffectContext<CreatureCard> where U : EventArgs {
    bool Evaluate(T context, U args);
}
#endregion
