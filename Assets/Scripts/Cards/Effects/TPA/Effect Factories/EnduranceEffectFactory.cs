public class EnduranceEffectFactory {
    public static StaticCreatureCardEffect Create(string id, string effectName) {
        EffectContext<CreatureCard> context = new EffectContext<CreatureCard>(id, effectName);

        EffectRule<EffectContext<CreatureCard>, PlayerCardCancelableEventArgs<CreatureCard>, CreatureCard> enduranceRule = new(context);
        enduranceRule.AddPrecondition(new CardCheckPrecondition<CreatureCard>());
        enduranceRule.AddAction(new CancelEventAction());

        EffectSequence<EffectContext<CreatureCard>, PlayerCardCancelableEventArgs<CreatureCard>, CreatureCard> enduranceSequence = new(new CreatureTappedTrigger());
        enduranceSequence.AddRule(enduranceRule);

        string rawDescription = "Attacking does not cause this creature to tap";
        StaticCreatureCardEffect elusiveEffect = new StaticCreatureCardEffect(rawDescription, effectIconId: "swords");
        elusiveEffect.AddEffectSequence(enduranceSequence);

        return elusiveEffect;
    }
}
