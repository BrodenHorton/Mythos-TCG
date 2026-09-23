using System;

public class StatBoostEffectFactory {
    public static CardEffect<CreatureCard> Create<TEventArgs>(string effectName,
                                                              string rawDescription,
                                                              StatBoostContext context,
                                                              EffectSequence<StatBoostContext, TEventArgs, CreatureCard> statBoostProkSequence) where TEventArgs : EventArgs {
        EffectRule<StatBoostContext, PlayerCardStatEventArgs<CreatureCard>, CreatureCard> attackCalcRule = new(context);
        attackCalcRule.AddPrecondition(new CardCheckPrecondition<CreatureCard>());
        attackCalcRule.AddPrecondition(new StatBoostCanBoostAttackPrecondition());
        attackCalcRule.AddAction(new StatBoostUpdateAttackCalcAction());
        
        EffectSequence<StatBoostContext, PlayerCardStatEventArgs<CreatureCard>, CreatureCard> attackCalcSequence = new(new CalculateCreatureAttackTrigger());
        attackCalcSequence.AddRule(attackCalcRule);

        EffectRule<StatBoostContext, PlayerCardStatEventArgs<CreatureCard>, CreatureCard> healthCalcRule = new(context);
        healthCalcRule.AddPrecondition(new CardCheckPrecondition<CreatureCard>());
        healthCalcRule.AddPrecondition(new StatBoostCanBoostHealthPrecondition());
        healthCalcRule.AddAction(new StatBoostUpdateHealthCalcAction());
        
        EffectSequence<StatBoostContext, PlayerCardStatEventArgs<CreatureCard>, CreatureCard> healthCalcSequence = new(new CalculateCreatureHealthTrigger());
        healthCalcSequence.AddRule(healthCalcRule);

        EffectRule<StatBoostContext, PlayerEventArgs, CreatureCard> clearEffectProksRule = new(context);
        clearEffectProksRule.AddPrecondition(new PlayerCheckPrecondition<CreatureCard>());
        clearEffectProksRule.AddPrecondition(new StatBoostIsResetAfterTurnPrecondition());
        clearEffectProksRule.AddAction(new StatBoostClearEffectProksAction());

        EffectSequence<StatBoostContext, PlayerEventArgs, CreatureCard> clearEffectProksSequence = new(new EndPhaseEnteredFinishedTrigger());
        clearEffectProksSequence.AddRule(clearEffectProksRule);

        CardEffect<CreatureCard> statBoostEffect = new CardEffect<CreatureCard>(effectName, rawDescription);
        statBoostEffect.AddEffectSequence(statBoostProkSequence);
        statBoostEffect.AddEffectSequence(attackCalcSequence);
        statBoostEffect.AddEffectSequence(healthCalcSequence);
        statBoostEffect.AddEffectSequence(clearEffectProksSequence);

        return statBoostEffect;
    }
}
