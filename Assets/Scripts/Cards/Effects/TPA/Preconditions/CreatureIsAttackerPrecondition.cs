public class CreatureIsAttackerPrecondition : EffectPrecondition<EffectContext<CreatureCard>, CreatureCombatDamageEventArgs, CreatureCard> {
    public bool Evaluate(EffectContext<CreatureCard> context, CreatureCombatDamageEventArgs args) {
        return args.Attacker != null && args.Attacker.Uuid == context.Card.Uuid;
    }
}
