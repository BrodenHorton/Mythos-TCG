using System;
using System.Collections.Generic;

#region TPA Base Classes
public abstract class EffectTrigger<T> where T : EventArgs {
    public event EventHandler<T> OnTriggerEffect;

    public abstract void Init();

    public abstract void RemoveListeners();

    protected void InvokeOnTriggerEffect(object sender, T args) {
        OnTriggerEffect?.Invoke(sender, args);
    }
}

public interface EffectPrecondition<in T, in U> where T : EffectContext<CreatureCard> where U : EventArgs {
    bool Evaluate(T context, U args);
}

public interface EffectAction<in T, in U> where T : EffectContext<CreatureCard> where U : EventArgs {
    void Execute(T context, U args);
}

public abstract class EffectContext<TCard> where TCard : Card {
    private string id;
    private string effectName;
    private TCard card;

    public EffectContext(string id, string effectName) {
        this.id = id;
        this.effectName = effectName;
    }

    public abstract void Init(TCard card);

    public abstract void RemoveListeners();

    public string Id { get { return id; } }

    public string EffectName { get { return effectName; } }

    public TCard Card { get { return card; } }
}

public interface IEffectSequence<TCard> {
    void Init(TCard card);

    void RemoveListeners();
}
#endregion

#region Triggers
public class LifePointsChangedTrigger : EffectTrigger<LifePointsChangedEventArgs> {
    public override void Init() {
        EventBus.Instance.OnLifePointsChanged += InvokeOnTriggerEffect;
    }

    public override void RemoveListeners() {
        EventBus.Instance.OnLifePointsChanged -= InvokeOnTriggerEffect;
    }
}
#endregion

#region Preconditions
public class LifePointsIncreasedPrecondition : EffectPrecondition<EffectContext<CreatureCard>, LifePointsChangedEventArgs> {
    public bool Evaluate(EffectContext<CreatureCard> context, LifePointsChangedEventArgs args) {
        return args.LifePoints > args.PreviousLifePoints;
    }
}
#endregion

#region Actions
public class StatBoostAction : EffectAction<StatBoostEffectContext, EventArgs> {
    public void Execute(StatBoostEffectContext effect, EventArgs args) {
        effect.effectProkCount++;
    }
}
#endregion

#region Context
public class StatBoostEffectContext : EffectContext<CreatureCard> {
    public int effectProkCount;

    public StatBoostEffectContext(string id, string effectName) : base(id, effectName) {
        effectProkCount = 0;
    }

    public override void Init(CreatureCard card) {
        throw new NotImplementedException();
    }

    public override void RemoveListeners() {
        throw new NotImplementedException();
    }
}
#endregion

#region Sequence
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

    private void TriggerHandler(object sender, UEventArgs args) {
        for (int i = 0; i < rules.Count; i++)
            rules[i].ExecuteRule(args);
    }
}
#endregion

#region Rule
public class EffectRule<TContext, UEventArgs> where TContext : EffectContext<CreatureCard> where UEventArgs : EventArgs {
    private List<EffectPrecondition<TContext, UEventArgs>> preconditions;
    private EffectAction<TContext, UEventArgs> action;
    private TContext context;

    public EffectRule() {

    }

    public void Init(CreatureCard card) {
        context.Init(card);
    }

    public void RemoveListeners() {
        context.RemoveListeners();
    }

    public void ExecuteRule(UEventArgs args) {
        for (int i = 0; i < preconditions.Count; i++) {
            if (!preconditions[i].Evaluate(context, args))
                return;
        }

        action.Execute(context, args);
    }
}
#endregion

#region Unique Card Effect
public class UniqueCardEffect<TCard> : CreatureCardEffect where TCard : Card {
    private List<IEffectSequence<CreatureCard>> sequences;
    private string rawDescription;

    public UniqueCardEffect(string rawDescription) {
        this.rawDescription = rawDescription;
    }

    public override void Init(CreatureCard card) {
        for(int i = 0; i < sequences.Count; i++)
            sequences[i].Init(card);
    }

    public override void RemoveListeners() {
        for (int i = 0; i < sequences.Count; i++)
            sequences[i].RemoveListeners();
    }

    public void AddEffectSequence(IEffectSequence<CreatureCard> sequence) {
        sequences.Add(sequence);
    }

    public override string GetRawDescription() {
        return rawDescription;
    }

    // Remove method
    public override CreatureCardEffectBase GetCreatureEffectBase() {
        throw new NotImplementedException();
    }

    public override CreatureCardEffectPayload GetEffectPayload() {
        throw new NotImplementedException();
    }
}
#endregion
