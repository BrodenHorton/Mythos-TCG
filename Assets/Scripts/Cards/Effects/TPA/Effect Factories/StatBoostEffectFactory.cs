using System;

public class StatBoostEffectFactory {
    public static CardEffect<CreatureCard> Create<TEventArgs>(StatBoostContext context,
                                                                    EffectSequence<StatBoostContext, TEventArgs> statBoostProkSequence,
                                                                    string rawDescription) where TEventArgs : EventArgs {
        EffectRule<StatBoostContext, PlayerCardStatEventArgs<CreatureCard>> attackCalcRule = new(context);
        attackCalcRule.AddPrecondition(new CardCheckPrecondition());
        attackCalcRule.AddPrecondition(new StatBoostCanBoostAttackPrecondition());
        attackCalcRule.AddAction(new StatBoostUpdateAttackCalceAction());
        EffectSequence<StatBoostContext, PlayerCardStatEventArgs<CreatureCard>> attackCalcSequence = new(new CalculateCreatureAttackTrigger());
        attackCalcSequence.AddRule(attackCalcRule);

        EffectRule<StatBoostContext, PlayerCardStatEventArgs<CreatureCard>> healthCalcRule = new(context);
        healthCalcRule.AddPrecondition(new CardCheckPrecondition());
        healthCalcRule.AddPrecondition(new StatBoostCanBoostHealthPrecondition());
        healthCalcRule.AddAction(new StatBoostUpdateHealthCalceAction());
        EffectSequence<StatBoostContext, PlayerCardStatEventArgs<CreatureCard>> healthCalcSequence = new(new CalculateCreatureHealthTrigger());
        healthCalcSequence.AddRule(healthCalcRule);

        EffectRule<StatBoostContext, PlayerEventArgs> clearEffectProksRule = new(context);
        clearEffectProksRule.AddPrecondition(new PlayerCheckPrecondition());
        clearEffectProksRule.AddPrecondition(new StatBoostIsResetAfterTurnPrecondition());
        clearEffectProksRule.AddAction(new StatBoostClearEffectProksAction());
        EffectSequence<StatBoostContext, PlayerEventArgs> clearEffectProksSequence = new(new EndPhaseEnteredFinishedTrigger());
        clearEffectProksSequence.AddRule(clearEffectProksRule);

        CardEffect<CreatureCard> statBoostEffect = new CardEffect<CreatureCard>(rawDescription);
        statBoostEffect.AddEffectSequence(statBoostProkSequence);
        statBoostEffect.AddEffectSequence(attackCalcSequence);
        statBoostEffect.AddEffectSequence(healthCalcSequence);
        statBoostEffect.AddEffectSequence(clearEffectProksSequence);

        return statBoostEffect;
    }
}
