using System;

public interface EffectPrecondition<in TContext, in UEventArgs> where TContext : EffectContext<CreatureCard> where UEventArgs : EventArgs {
    bool Evaluate(TContext context, UEventArgs args);
}
