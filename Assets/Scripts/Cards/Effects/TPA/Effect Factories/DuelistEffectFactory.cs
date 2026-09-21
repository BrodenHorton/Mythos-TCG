public class DuelistEffectFactory {
    public static StaticCreatureCardEffect Create(string id, string effectName) {
        DuelistContext context = new(id, effectName);

        EffectRule<DuelistContext, SelectableCardsEventArgs, CreatureCard> setTargetCardsSelectableRule = new(context);
        setTargetCardsSelectableRule.AddPrecondition(new PlayerCheckPrecondition<CreatureCard>());
        setTargetCardsSelectableRule.AddPrecondition(new PlayerTurnCheckPrecondition<CreatureCard>());
        setTargetCardsSelectableRule.AddPrecondition(new CombatStateCheckPrecondition<CreatureCard>());
        setTargetCardsSelectableRule.AddPrecondition(new CreatureInCombatPrecondition());
        setTargetCardsSelectableRule.AddPrecondition(new CreatureInCombatWithDefenderPrecondition(), shouldEvaluateAsNot: true);
        setTargetCardsSelectableRule.AddAction(new SetTargetCardsSelectableAction());

        EffectSequence<DuelistContext, SelectableCardsEventArgs, CreatureCard> setTargetCardsSelectableSequence = new(new GetSelectableCardsTrigger());
        setTargetCardsSelectableSequence.AddRule(setTargetCardsSelectableRule);

        EffectRule<DuelistContext, CreatureReleasedOverCreatureEventArgs, CreatureCard> setDuelistDefenderRule = new(context);
        setDuelistDefenderRule.AddPrecondition(new PlayerCheckPrecondition<CreatureCard>());
        setDuelistDefenderRule.AddPrecondition(new PlayerTurnCheckPrecondition<CreatureCard>());
        setDuelistDefenderRule.AddPrecondition(new CombatStateCheckPrecondition<CreatureCard>());
        setDuelistDefenderRule.AddPrecondition(new DuelistCreatureOverCreatureHoverPrecondition());
        setDuelistDefenderRule.AddAction(new SetDuelistsDefendereAction());

        EffectSequence<DuelistContext, CreatureReleasedOverCreatureEventArgs, CreatureCard> setDuelistDefenderSequence = new(new CreatureReleasedOverCreatureTrigger());
        setDuelistDefenderSequence.AddRule(setDuelistDefenderRule);

        EffectRule<DuelistContext, SelectableCardsEventArgs, CreatureCard> removeDuelistTargetFromSelectableCardsRule = new(context);
        removeDuelistTargetFromSelectableCardsRule.AddPrecondition(new RemoveDuelistTargetFromSelectableCardsPrecondition());
        removeDuelistTargetFromSelectableCardsRule.AddAction(new RemoveDuelistTargetFromSelectableCardsAction());

        EffectSequence<DuelistContext, SelectableCardsEventArgs, CreatureCard> removeDuelistTargetFromSelectableCardsSequence = new(new GetSelectableCardsTrigger());
        removeDuelistTargetFromSelectableCardsSequence.AddRule(removeDuelistTargetFromSelectableCardsRule);

        EffectRule<DuelistContext, CombatCreatureEventArgs, CreatureCard> clearDuelistDefenderRule = new(context);
        clearDuelistDefenderRule.AddPrecondition(new PlayerIsInitiatorPrecondition());
        clearDuelistDefenderRule.AddPrecondition(new DuelistDefenderExistsPrecondition());
        clearDuelistDefenderRule.AddPrecondition(new TargetIsDuelistDefenderPlayerPrecondition());
        clearDuelistDefenderRule.AddAction(new ClearDuelistDefenderAction());

        EffectSequence<DuelistContext, CombatCreatureEventArgs, CreatureCard> clearDuelistDefenderSequence = new(new UndeclareAttackerTrigger());
        clearDuelistDefenderSequence.AddRule(clearDuelistDefenderRule);

        string rawDescription = "When this creature attacks, choose the enemy creature that defends";
        StaticCreatureCardEffect duelistEffect = new StaticCreatureCardEffect(rawDescription, effectIconId: "swords");
        duelistEffect.AddEffectSequence(setTargetCardsSelectableSequence);
        duelistEffect.AddEffectSequence(setDuelistDefenderSequence);
        duelistEffect.AddEffectSequence(removeDuelistTargetFromSelectableCardsSequence);
        duelistEffect.AddEffectSequence(clearDuelistDefenderSequence);

        return duelistEffect;
    }
}