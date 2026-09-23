using System;
using System.Collections.Generic;

public class EffectSequence<TContext, UEventArgs, VCard> : IEffectSequence<CreatureCard> where TContext : EffectContext<VCard> where UEventArgs : EventArgs where VCard : Card {
    private EffectTrigger<UEventArgs> trigger;
    private List<EffectRule<TContext, UEventArgs, VCard>> rules;

    public EffectSequence(EffectTrigger<UEventArgs> trigger) {
        this.trigger = trigger;
        rules = new List<EffectRule<TContext, UEventArgs, VCard>>();
    }

    public void Init(CreatureCard card) {
        trigger.Init();
        trigger.OnTriggerEffect += TriggerHandler;
    }

    public void RemoveListeners() {
        trigger.RemoveListeners();
        trigger.OnTriggerEffect -= TriggerHandler;
    }

    public void AddRule(EffectRule<TContext, UEventArgs, VCard> rule) {
        rules.Add(rule);
    }

    private void TriggerHandler(object sender, UEventArgs args) {
        for (int i = 0; i < rules.Count; i++)
            rules[i].ExecuteRule(args);
    }
}