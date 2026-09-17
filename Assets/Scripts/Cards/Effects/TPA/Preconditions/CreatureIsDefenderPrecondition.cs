public class CreatureIsDefenderPrecondition : EffectPrecondition<EffectContext<CreatureCard>, CreatureCombatEventArgs, CreatureCard> {
    public bool Evaluate(EffectContext<CreatureCard> context, CreatureCombatEventArgs args) {
        return args.Defender != null && args.Defender.Uuid == context.Card.Uuid;
    }
}
