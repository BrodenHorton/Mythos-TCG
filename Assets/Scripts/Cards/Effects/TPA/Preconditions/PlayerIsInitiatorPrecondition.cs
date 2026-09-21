public class PlayerIsInitiatorPrecondition : EffectPrecondition<EffectContext<CreatureCard>, CombatCreatureEventArgs, CreatureCard> {
    public bool Evaluate(EffectContext<CreatureCard> context, CombatCreatureEventArgs args) {
        return context.Card.PlayerId == args.InitiatorId;
    }
}