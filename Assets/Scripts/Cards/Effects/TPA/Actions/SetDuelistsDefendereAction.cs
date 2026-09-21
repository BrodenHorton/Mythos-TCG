public class SetDuelistsDefendereAction : EffectAction<DuelistContext, CreatureReleasedOverCreatureEventArgs, CreatureCard> {
    private CombatManager combatManager;

    public SetDuelistsDefendereAction() {
        combatManager = ServiceLocator.Get<CombatManager>();
    }

    public void Execute(DuelistContext context, CreatureReleasedOverCreatureEventArgs args) {
        TcgLogger.Log("Duelist Set Targets Activated");
        context.DuelistDefender = args.HeldCard;
        combatManager.DeclareDefender(args.HeldCard.PlayerId, args.HoveredCard, args.HeldCard);
    }
}