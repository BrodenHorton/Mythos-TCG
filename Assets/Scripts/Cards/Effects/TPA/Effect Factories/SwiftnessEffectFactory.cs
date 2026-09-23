public class SwiftnessEffectFactory {
    public static StaticCreatureCardEffect Create() {
        EffectContext<CreatureCard> context = new();

        EffectRule<EffectContext<CreatureCard>, PlayerCardCancelableEventArgs<CreatureCard>, CreatureCard> lifelinkRule = new(context);
        lifelinkRule.AddPrecondition(new CardCheckPrecondition<CreatureCard>());
        lifelinkRule.AddAction(new CancelEventAction());

        EffectSequence<EffectContext<CreatureCard>, PlayerCardCancelableEventArgs<CreatureCard>, CreatureCard> lifelinkSequence = new(new SummoningSicknessTrigger());
        lifelinkSequence.AddRule(lifelinkRule);

        string rawDescription = "This creature does not have summoning sickness";
        StaticCreatureCardEffect lifelinkEffect = new StaticCreatureCardEffect(effectName: "Swiftness",
                                                                              rawDescription,
                                                                              effectIconId: "swords");
        lifelinkEffect.AddEffectSequence(lifelinkSequence);

        return lifelinkEffect;
    }
}