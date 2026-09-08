using System;
using System.Collections.Generic;

public class EffectSequence<TContext, UEventArgs> : IEffectSequence<CreatureCard> where TContext : EffectContext<CreatureCard> where UEventArgs : EventArgs {
    private EffectTrigger<UEventArgs> trigger;
    private List<EffectRule<TContext, UEventArgs>> rules;

    public void Init(CreatureCard card) {
        trigger.Init();
        trigger.OnTriggerEffect += TriggerHandler;

        for(int i = 0; i < rules.Count; i++)
            rules[i].Init(card);
    }

    public void RemoveListeners() {
        trigger.RemoveListeners();
        trigger.OnTriggerEffect -= TriggerHandler;

        for (int i = 0; i < rules.Count; i++)
            rules[i].RemoveListeners();
    }

    public void SetTrigger(EffectTrigger<UEventArgs> trigger) {
        this.trigger = trigger;
    }

    public void AddRule(EffectRule<TContext, UEventArgs> rule) {
        rules.Add(rule);
    }

    private void TriggerHandler(object sender, UEventArgs args) {
        for (int i = 0; i < rules.Count; i++)
            rules[i].ExecuteRule(args);
    }
}