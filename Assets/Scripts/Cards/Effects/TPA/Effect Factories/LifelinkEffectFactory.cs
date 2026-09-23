public class LifelinkEffectFactory {
    public static StaticCreatureCardEffect Create() {
        EffectContext<CreatureCard> context = new();

        EffectRule<EffectContext<CreatureCard>, CreatureCombatDamageEventArgs, CreatureCard> lifelinkRule = new(context);
        lifelinkRule.AddPrecondition(new CreatureIsAttackerPrecondition());
        lifelinkRule.AddPrecondition(new DamageDealtPrecondition());
        lifelinkRule.AddAction(new LifelinkAction());

        EffectSequence<EffectContext<CreatureCard>, CreatureCombatDamageEventArgs, CreatureCard> lifelinkSequence = new(new CreatureCombatFinishedTrigger());
        lifelinkSequence.AddRule(lifelinkRule);

        string rawDescription = "Increase life points equal to the damage dealt to the defender";
        StaticCreatureCardEffect lifelinkEffect = new StaticCreatureCardEffect(effectName: "Lifelink",
                                                                              rawDescription,
                                                                              effectIconId: "swords");
        lifelinkEffect.AddEffectSequence(lifelinkSequence);

        return lifelinkEffect;
    }
}
