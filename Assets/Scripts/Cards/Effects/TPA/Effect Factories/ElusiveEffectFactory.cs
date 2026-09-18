public class ElusiveEffectFactory {
    public static StaticCreatureCardEffect Create(string id, string effectName) {
        EffectContext<CreatureCard> context = new EffectContext<CreatureCard>(id, effectName);

        EffectRule<EffectContext<CreatureCard>, CanDefendEventArgs, CreatureCard> restrictDefendersRule = new(context);
        restrictDefendersRule.AddPrecondition(new CreatureIsAttackerPrecondition());
        restrictDefendersRule.AddPrecondition(new CanTargetDefendPrecondition());
        restrictDefendersRule.AddAction(new RestrictDefendersAction());

        EffectSequence<EffectContext<CreatureCard>, CanDefendEventArgs, CreatureCard> elusiveSequence = new(new SelectAttackerToDefendTrigger());
        elusiveSequence.AddRule(restrictDefendersRule);

        EffectRule<EffectContext<CreatureCard>, CanDefendEventArgs, CreatureCard> canDefendElusiveAttackerRule = new(context);
        canDefendElusiveAttackerRule.AddPrecondition(new CreatureIsDefenderPrecondition());
        canDefendElusiveAttackerRule.AddAction(new SetCanDefendAction(canDefend: true));

        EffectSequence<EffectContext<CreatureCard>, CanDefendEventArgs, CreatureCard> canDefendElusiveAttackerSequence = new(new SelectElusiveAttackerToDefendTrigger());
        canDefendElusiveAttackerSequence.AddRule(canDefendElusiveAttackerRule);

        string rawDescription = "Can only be blocked by creatures with Elusive or Reach";
        StaticCreatureCardEffect elusiveEffect = new StaticCreatureCardEffect(rawDescription, effectIconId: "swords");
        elusiveEffect.AddEffectSequence(elusiveSequence);
        elusiveEffect.AddEffectSequence(canDefendElusiveAttackerSequence);

        return elusiveEffect;
    }
}
