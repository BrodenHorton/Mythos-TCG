public class TargetIsDuelistDefenderPlayerPrecondition : EffectPrecondition<DuelistContext, CombatCreatureEventArgs, CreatureCard> {
    public bool Evaluate(DuelistContext context, CombatCreatureEventArgs args) {
        return args.TargetId == context.DuelistDefender.PlayerId;
    }
}