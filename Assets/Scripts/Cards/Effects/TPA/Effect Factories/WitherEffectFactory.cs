public class WitherEffectFactory {
    public static StaticCreatureCardEffect Create(string id, string effectName) {
        EffectContext<CreatureCard> context = new(id, effectName);

        EffectRule<EffectContext<CreatureCard>, CreatureCombatDamageEventArgs, CreatureCard> stopDamageToDefenderRule = new(context);
        stopDamageToDefenderRule.AddPrecondition(new CreatureIsAttackerPrecondition());
        stopDamageToDefenderRule.AddPrecondition(new CreatureCombatDamageEventActivedPrecondition());
        stopDamageToDefenderRule.AddPrecondition(new DefenderExistsPrecondition());
        stopDamageToDefenderRule.AddAction(new SetShouldDamageDefenderAction(shouldDamageDefender: false));

        EffectSequence<EffectContext<CreatureCard>, CreatureCombatDamageEventArgs, CreatureCard> stopDamageToDefenderSequence = new(new CreatureDamagedByCreatureTrigger());
        stopDamageToDefenderSequence.AddRule(stopDamageToDefenderRule);

        EffectRule<EffectContext<CreatureCard>, CreatureCombatDamageEventArgs, CreatureCard> addWitherStatusRule = new(context);
        addWitherStatusRule.AddPrecondition(new CreatureIsAttackerPrecondition());
        addWitherStatusRule.AddPrecondition(new DefenderExistsPrecondition());
        addWitherStatusRule.AddAction(new AddWitherStatusAction());

        EffectSequence<EffectContext<CreatureCard>, CreatureCombatDamageEventArgs, CreatureCard> addWitherStatusSequence = new(new CreatureDamagedByCreatureFinishedTrigger());
        addWitherStatusSequence.AddRule(addWitherStatusRule);

        string rawDescription = "Deals damage as -1/-1 debuffs";
        StaticCreatureCardEffect witherEffect = new StaticCreatureCardEffect(rawDescription, effectIconId: "swords");
        witherEffect.AddEffectSequence(stopDamageToDefenderSequence);
        witherEffect.AddEffectSequence(addWitherStatusSequence);

        return witherEffect;
    }
}
