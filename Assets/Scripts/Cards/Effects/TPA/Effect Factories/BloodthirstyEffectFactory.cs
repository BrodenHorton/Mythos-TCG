public class BloodthirstyEffectFactory {
    public static StaticCreatureCardEffect Create() {
        StatBoostContext context = new StatBoostContext(atkBoost: 1,
                                                        healthBoost: 1);

        EffectRule<StatBoostContext, CreatureCombatDamageEventArgs, CreatureCard> bloodthirstyRule = new(context);
        bloodthirstyRule.AddPrecondition(new CreatureIsAttackerPrecondition());
        bloodthirstyRule.AddAction(new StatBoostIncrementAction());

        EffectSequence<StatBoostContext, CreatureCombatDamageEventArgs, CreatureCard> bloodthirstySequence = new(new CreatureDamagedByCreatureTrigger());
        bloodthirstySequence.AddRule(bloodthirstyRule);

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

        string rawDescription = "When this creature deals damage, it gains +1/+1";
        StaticCreatureCardEffect bloodthirstyEffect = new StaticCreatureCardEffect(effectName: "Bloodthirstry",
                                                                                   rawDescription,
                                                                                   effectIconId: "swords");
        bloodthirstyEffect.AddEffectSequence(bloodthirstySequence);
        bloodthirstyEffect.AddEffectSequence(attackCalcSequence);
        bloodthirstyEffect.AddEffectSequence(healthCalcSequence);

        return bloodthirstyEffect;
    }
}
