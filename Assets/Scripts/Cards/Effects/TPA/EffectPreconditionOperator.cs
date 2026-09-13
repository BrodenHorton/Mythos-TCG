using System;

public class EffectPreconditionOperator<TContext, UEventArgs> where TContext : EffectContext<CreatureCard> where UEventArgs : EventArgs {
    private EffectPrecondition<TContext, UEventArgs> precondition;
    private bool shouldEvaluateAsNot;

    public EffectPreconditionOperator(EffectPrecondition<TContext, UEventArgs> precondition, bool shouldEvaluateAsNot) {
        this.precondition = precondition;
        this.shouldEvaluateAsNot = shouldEvaluateAsNot;
    }

    public bool Evaluate(TContext context, UEventArgs args) {
        return !shouldEvaluateAsNot == precondition.Evaluate(context, args);
    }

    public EffectPrecondition<TContext, UEventArgs> Precondition { get { return precondition; } }

    public bool ShouldEvaluateAsNot { get { return shouldEvaluateAsNot; } }
}