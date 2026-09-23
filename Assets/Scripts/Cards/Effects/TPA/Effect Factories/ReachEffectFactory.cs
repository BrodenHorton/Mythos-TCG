public class ReachEffectFactory {
    public static StaticCreatureCardEffect Create() {
        EffectContext<CreatureCard> context = new();

        EffectRule<EffectContext<CreatureCard>, CanDefendEventArgs, CreatureCard> reachRule = new(context);
        reachRule.AddPrecondition(new CreatureIsDefenderPrecondition());
        reachRule.AddAction(new SetCanDefendAction(canDefend: true));

        EffectSequence<EffectContext<CreatureCard>, CanDefendEventArgs, CreatureCard> reachSequence = new(new SelectElusiveAttackerToDefendTrigger());
        reachSequence.AddRule(reachRule);

        string rawDescription = "Can block creatures with Elusive";
        StaticCreatureCardEffect reachEffect = new StaticCreatureCardEffect(effectName: "Reach",
                                                                              rawDescription,
                                                                              effectIconId: "swords");
        reachEffect.AddEffectSequence(reachSequence);

        return reachEffect;
    }
}