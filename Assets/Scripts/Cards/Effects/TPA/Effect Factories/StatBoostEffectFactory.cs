
using System;

public class StatBoostEffectFactory {
        
    public static UniqueCardEffect<CreatureCard> Create<TEventArgs>(StatBoostEffectContext context,
                                                                    EffectSequence<StatBoostEffectContext, TEventArgs> statBoostProkSequence,
                                                                    string rawDescription) where TEventArgs : EventArgs {
        EffectRule<StatBoostEffectContext, PlayerCardStatEventArgs<CreatureCard>> attackCalcRule = new(context);
        attackCalcRule.AddPrecondition(new CardCheckPrecondition());
        attackCalcRule.AddPrecondition(new StatBoostCanBoostAttackPrecondition());
        attackCalcRule.AddAction(new StatBoostUpdateAttackCalceAction());
        EffectSequence<StatBoostEffectContext, PlayerCardStatEventArgs<CreatureCard>> attackCalcSequence = new(new CalculateCreatureAttackTrigger());
        attackCalcSequence.AddRule(attackCalcRule);

        EffectRule<StatBoostEffectContext, PlayerCardStatEventArgs<CreatureCard>> healthCalcRule = new(context);
        healthCalcRule.AddPrecondition(new CardCheckPrecondition());
        healthCalcRule.AddPrecondition(new StatBoostCanBoostHealthPrecondition());
        healthCalcRule.AddAction(new StatBoostUpdateHealthCalceAction());
        EffectSequence<StatBoostEffectContext, PlayerCardStatEventArgs<CreatureCard>> healthCalcSequence = new(new CalculateCreatureHealthTrigger());
        healthCalcSequence.AddRule(healthCalcRule);

        EffectRule<StatBoostEffectContext, PlayerEventArgs> clearEffectProksRule = new(context);
        clearEffectProksRule.AddPrecondition(new PlayerCheckPrecondition());
        clearEffectProksRule.AddPrecondition(new StatBoostIsResetAfterTurnPrecondition());
        clearEffectProksRule.AddAction(new StatBoostClearEffectProksAction());
        EffectSequence<StatBoostEffectContext, PlayerEventArgs> clearEffectProksSequence = new(new EndPhaseEnteredFinishedTrigger());
        clearEffectProksSequence.AddRule(clearEffectProksRule);

        UniqueCardEffect<CreatureCard> statBoostEffect = new UniqueCardEffect<CreatureCard>(rawDescription);
        statBoostEffect.AddEffectSequence(statBoostProkSequence);
        statBoostEffect.AddEffectSequence(attackCalcSequence);
        statBoostEffect.AddEffectSequence(healthCalcSequence);
        statBoostEffect.AddEffectSequence(clearEffectProksSequence);

        return statBoostEffect;
    }
}
