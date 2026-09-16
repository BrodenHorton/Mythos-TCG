using System;

public interface EffectPrecondition<in TContext, in UEventArgs, VCard> where TContext : EffectContext<VCard> where UEventArgs : EventArgs where VCard : Card {
    bool Evaluate(TContext context, UEventArgs args);
}
