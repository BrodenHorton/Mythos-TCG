using System;

public interface EffectAction<in T, in U> where T : EffectContext<CreatureCard> where U : EventArgs {
    void Execute(T context, U args);
}
#endregion
