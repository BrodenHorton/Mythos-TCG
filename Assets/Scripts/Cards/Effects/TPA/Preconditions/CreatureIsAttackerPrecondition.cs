public class CreatureIsAttackerPrecondition : EffectPrecondition<EffectContext<CreatureCard>, CreatureCombatDamageEventArgs> {
    public bool Evaluate(EffectContext<CreatureCard> context, CreatureCombatDamageEventArgs args) {
        return args.Attacker.Uuid == context.Card.Uuid;
    }
}
