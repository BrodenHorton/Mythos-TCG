using System;
using System.Collections.Generic;

public class EffectRule<TContext, UEventArgs, VCard> where TContext : EffectContext<VCard> where UEventArgs : EventArgs where VCard : Card {
    private List<EffectPreconditionOperator<TContext, UEventArgs, VCard>> preconditions;
    private EffectAction<TContext, UEventArgs, VCard> action;
    private TContext context;

    public EffectRule(TContext context) {
        preconditions = new List<EffectPreconditionOperator<TContext, UEventArgs, VCard>>();
        this.context = context;
    }

    public void AddAction(EffectAction<TContext, UEventArgs, VCard> action) {
        this.action = action;
    }

    public void AddPrecondition(EffectPrecondition<TContext, UEventArgs, VCard> precondtion, bool shouldEvaluateAsNot = false) {
        preconditions.Add(new EffectPreconditionOperator<TContext, UEventArgs, VCard>(precondtion, shouldEvaluateAsNot));
    }

    public void ExecuteRule(UEventArgs args) {
        for (int i = 0; i < preconditions.Count; i++) {
            if (!preconditions[i].Evaluate(context, args))
                return;
        }

        action.Execute(context, args);
    }
}
