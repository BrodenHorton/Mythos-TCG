public class DefenderEffectFactory {
    public static StaticCreatureCardEffect Create(string id, string effectName) {
        EffectContext<CreatureCard> context = new EffectContext<CreatureCard>(id, effectName);

        EffectRule<EffectContext<CreatureCard>, PlayerCardCancelableEventArgs<CreatureCard>, CreatureCard> defenderRule = new(context);
        defenderRule.AddPrecondition(new CardCheckPrecondition<CreatureCard>());
        defenderRule.AddAction(new CancelEventAction());

        EffectSequence<EffectContext<CreatureCard>, PlayerCardCancelableEventArgs<CreatureCard>, CreatureCard> defenderSequence = new(new CanCreatureAttackTrigger());
        defenderSequence.AddRule(defenderRule);

        string rawDescription = "This creature cannot declare an attack";
        StaticCreatureCardEffect defenderEffect = new StaticCreatureCardEffect(rawDescription, effectIconId: "swords");
        defenderEffect.AddEffectSequence(defenderSequence);

        return defenderEffect;
    }
}