using System;

public class EffectPreconditionOperator<TContext, UEventArgs, VCard> where TContext : EffectContext<VCard> where UEventArgs : EventArgs where VCard : Card {
    private EffectPrecondition<TContext, UEventArgs, VCard> precondition;
    private bool shouldEvaluateAsNot;

    public EffectPreconditionOperator(EffectPrecondition<TContext, UEventArgs, VCard> precondition, bool shouldEvaluateAsNot) {
        this.precondition = precondition;
        this.shouldEvaluateAsNot = shouldEvaluateAsNot;
    }

    public bool Evaluate(TContext context, UEventArgs args) {
        return !shouldEvaluateAsNot == precondition.Evaluate(context, args);
    }

    public EffectPrecondition<TContext, UEventArgs, VCard> Precondition { get { return precondition; } }

    public bool ShouldEvaluateAsNot { get { return shouldEvaluateAsNot; } }
}