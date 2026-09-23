public class WitherStatusEffectFactory {
    public static CardEffect<CreatureCard> Create() {
        WitherStatusContext context = new();

        EffectRule<WitherStatusContext, CreatureCombatDamageEventArgs, CreatureCard> witherProkedRule = new(context);
        witherProkedRule.AddPrecondition(new CreatureIsDefenderPrecondition());
        witherProkedRule.AddAction(new WitherProkedAction());

        EffectSequence<WitherStatusContext, CreatureCombatDamageEventArgs, CreatureCard> witherProkedSequence = new(new WitherProkedTrigger());
        witherProkedSequence.AddRule(witherProkedRule);

        EffectRule<WitherStatusContext, PlayerCardStatEventArgs<CreatureCard>, CreatureCard> attackCalcRule = new(context);
        attackCalcRule.AddPrecondition(new CardCheckPrecondition<CreatureCard>());
        attackCalcRule.AddPrecondition(new WitherStatusHasProksPrecondition());
        attackCalcRule.AddAction(new WitherStatustCalcUpdateAction());

        EffectSequence<WitherStatusContext, PlayerCardStatEventArgs<CreatureCard>, CreatureCard> attackCalcSequence = new(new CalculateCreatureAttackTrigger());
        attackCalcSequence.AddRule(attackCalcRule);

        EffectRule<WitherStatusContext, PlayerCardStatEventArgs<CreatureCard>, CreatureCard> healthCalcRule = new(context);
        healthCalcRule.AddPrecondition(new CardCheckPrecondition<CreatureCard>());
        healthCalcRule.AddPrecondition(new WitherStatusHasProksPrecondition());
        healthCalcRule.AddAction(new WitherStatustCalcUpdateAction());

        EffectSequence<WitherStatusContext, PlayerCardStatEventArgs<CreatureCard>, CreatureCard> healthCalcSequence = new(new CalculateCreatureHealthTrigger());
        healthCalcSequence.AddRule(healthCalcRule);

        string rawDescription = "Wither Status";
        StaticCreatureCardEffect witherEffect = new StaticCreatureCardEffect(effectName: "Wither Status",
                                                                              rawDescription,
                                                                              effectIconId: "swords");
        witherEffect.AddEffectSequence(witherProkedSequence);
        witherEffect.AddEffectSequence(attackCalcSequence);
        witherEffect.AddEffectSequence(healthCalcSequence);

        return witherEffect;
    }
}