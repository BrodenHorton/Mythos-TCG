public class OverwhelmEffectFactory {
    public static StaticCreatureCardEffect Create(EffectContext<CreatureCard> context, string rawDescription, string effectIconId) {
        EffectRule<EffectContext<CreatureCard>, CreatureCombatDamageEventArgs> overwhelmRule = new(context);
        overwhelmRule.AddPrecondition(new CreatureIsAttackerPrecondition());
        overwhelmRule.AddPrecondition(new CreatureCombatDamageEventActivedPrecondition());
        overwhelmRule.AddAction(new OverwhelmAction());

        EffectSequence<EffectContext<CreatureCard>, CreatureCombatDamageEventArgs> overwhelmSequence = new(new CreatureDamagedByCreatureTrigger());
        overwhelmSequence.AddRule(overwhelmRule);

        StaticCreatureCardEffect overwhelmEffect = new StaticCreatureCardEffect(rawDescription, effectIconId);
        overwhelmEffect.AddEffectSequence(overwhelmSequence);

        return overwhelmEffect;
    }
}