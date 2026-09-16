public class DestroyDefenderAction : EffectAction<EffectContext<CreatureCard>, CreatureCombatDamageEventArgs, CreatureCard> {
    public void Execute(EffectContext<CreatureCard> context, CreatureCombatDamageEventArgs args) {
        args.Defender.DestroyCreature();
    }
}
