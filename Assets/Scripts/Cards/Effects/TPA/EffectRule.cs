using System;
using System.Collections.Generic;

public class EffectRule<TContext, UEventArgs> where TContext : EffectContext<CreatureCard> where UEventArgs : EventArgs {
    private List<EffectPrecondition<TContext, UEventArgs>> preconditions;
    private EffectAction<TContext, UEventArgs> action;
    private TContext context;

    public EffectRule(TContext context) {
        preconditions = new List<EffectPrecondition<TContext, UEventArgs>>();
        this.context = context;
    }

    public void AddAction(EffectAction<TContext, UEventArgs> action) {
        this.action = action;
    }

    public void AddPrecondition(EffectPrecondition<TContext, UEventArgs> precondtion) {
        preconditions.Add(precondtion);
    }

    public void ExecuteRule(UEventArgs args) {
        for (int i = 0; i < preconditions.Count; i++) {
            if (!preconditions[i].Evaluate(context, args))
                return;
        }

        action.Execute(context, args);
    }
}
