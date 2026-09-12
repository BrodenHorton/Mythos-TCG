using System;

public interface EffectAction<in TContext, in UEventArgs> where TContext : EffectContext<CreatureCard> where UEventArgs : EventArgs {
    void Execute(TContext context, UEventArgs args);
}
