public class SetTargetCardsSelectableAction : EffectAction<EffectContext<CreatureCard>, SelectableCardsEventArgs, CreatureCard> {
    private DuelManager duelManager;
    private CombatManager combatManager;

    public SetTargetCardsSelectableAction() {
        duelManager = ServiceLocator.Get<DuelManager>();
        combatManager = ServiceLocator.Get<CombatManager>();
    }
    
    public void Execute(EffectContext<CreatureCard> context, SelectableCardsEventArgs args) {
        DuelistCombat duelistCombat = combatManager.GetDuelistCombat(context.Card.Uuid);
        MatchPlayer target = duelManager.GetPlayerById(duelistCombat.TargetId);
        foreach (CreatureCard creature in target.Creatures) {
            if (!combatManager.IsCreatureInCombat(creature.Uuid))
                args.CardUuids.Add(creature.Uuid);
        }
    }
}
