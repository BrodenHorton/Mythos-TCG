public class OverwhelmEffectFactory {
    public static StaticCreatureCardEffect Create() {
        EffectContext<CreatureCard> context = new();

        EffectRule<EffectContext<CreatureCard>, CreatureCombatDamageEventArgs, CreatureCard> overwhelmRule = new(context);
        overwhelmRule.AddPrecondition(new CreatureIsAttackerPrecondition());
        overwhelmRule.AddPrecondition(new CreatureCombatDamageEventActivedPrecondition());
        overwhelmRule.AddAction(new OverwhelmAction());

        EffectSequence<EffectContext<CreatureCard>, CreatureCombatDamageEventArgs, CreatureCard> overwhelmSequence = new(new CreatureDamagedByCreatureTrigger());
        overwhelmSequence.AddRule(overwhelmRule);

        string rawDescription = "Overflow damage that isn’t blocked by a defender's Health is dealt as life point damage";
        StaticCreatureCardEffect overwhelmEffect = new StaticCreatureCardEffect(effectName: "Overwhelm",
                                                                                rawDescription,
                                                                                effectIconId: "swords");
        overwhelmEffect.AddEffectSequence(overwhelmSequence);

        return overwhelmEffect;
    }
}