using System;

public interface EffectAction<in TContext, in UEventArgs, VCard> where TContext : EffectContext<VCard> where UEventArgs : EventArgs where VCard : Card {
    void Execute(TContext context, UEventArgs args);
}
