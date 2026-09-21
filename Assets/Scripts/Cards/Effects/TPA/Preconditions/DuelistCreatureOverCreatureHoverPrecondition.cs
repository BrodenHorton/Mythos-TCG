public class DuelistCreatureOverCreatureHoverPrecondition : EffectPrecondition<DuelistContext, CreatureReleasedOverCreatureEventArgs, CreatureCard> {
    private CombatManager combatManager;

    public DuelistCreatureOverCreatureHoverPrecondition() {
        combatManager = ServiceLocator.Get<CombatManager>();
    }

    public bool Evaluate(DuelistContext context, CreatureReleasedOverCreatureEventArgs args) {
        if (args.HoveredCard.Uuid != context.Card.Uuid)
            return false;
        if (!combatManager.HasExistingDuelistCombat(context.Card.PlayerId, args.HeldCard.PlayerId))
            return false;
        if (!combatManager.IsCreatureInCombat(context.Card.Uuid))
            return false;
        if (combatManager.IsCreatureInCombat(args.HeldCard.Uuid))
            return false;
        CreatureCombat creatureCombat = combatManager.GetCreatureCombat(context.Card.Uuid);
        if (creatureCombat.Defender != null)
            return false;

        return true;
    }
}
