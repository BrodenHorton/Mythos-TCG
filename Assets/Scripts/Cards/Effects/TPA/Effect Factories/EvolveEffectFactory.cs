public class EvolveEffectFactory {
    public static CardEffect<CreatureCard> Create(EvolveContext context, string rawDescription) {
        EffectRule<EvolveContext, PlayerEventArgs> evolveRule = new(context);
        evolveRule.AddPrecondition(new PlayerCheckPrecondition());
        evolveRule.AddAction(new EvolveIncrementAction());

        EffectSequence<EvolveContext, PlayerEventArgs> evolveSequence = new(new StartPhaseEnteredFinishedTrigger());
        evolveSequence.AddRule(evolveRule);

        CardEffect<CreatureCard> evolveEffect = new CardEffect<CreatureCard>(rawDescription);
        evolveEffect.AddEffectSequence(evolveSequence);

        return evolveEffect;
    }
}
