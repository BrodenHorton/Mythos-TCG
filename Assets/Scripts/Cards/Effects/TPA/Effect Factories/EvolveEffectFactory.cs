using static CardRichTextUtil;

public class EvolveEffectFactory {
    public static CardEffect<CreatureCard> Create(string effectName,
                                                  CreatureCardBase evolution,
                                                  int evolutionRequiredTurnCount) {
        EvolveContext context = new EvolveContext(evolution, evolutionRequiredTurnCount);

        EffectRule<EvolveContext, PlayerEventArgs, CreatureCard> evolveRule = new(context);
        evolveRule.AddPrecondition(new PlayerCheckPrecondition<CreatureCard>());
        evolveRule.AddAction(new EvolveIncrementAction());

        EffectSequence<EvolveContext, PlayerEventArgs, CreatureCard> evolveSequence = new(new StartPhaseEnteredFinishedTrigger());
        evolveSequence.AddRule(evolveRule);

        string rawDescription = GetKeywordLinkTagText("evolve", "Evolve " + context.EvolutionRequiredTurnCount) + ": " + context.Evolution.CardName;
        CardEffect<CreatureCard> evolveEffect = new CardEffect<CreatureCard>(effectName, rawDescription);
        evolveEffect.AddEffectSequence(evolveSequence);

        return evolveEffect;
    }
}
