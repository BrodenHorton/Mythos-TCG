public class CreatureIsAttackerPrecondition : EffectPrecondition<EffectContext<CreatureCard>, CreatureCombatEventArgs, CreatureCard> {
    public bool Evaluate(EffectContext<CreatureCard> context, CreatureCombatEventArgs args) {
        return args.Attacker != null && args.Attacker.Uuid == context.Card.Uuid;
    }
}
