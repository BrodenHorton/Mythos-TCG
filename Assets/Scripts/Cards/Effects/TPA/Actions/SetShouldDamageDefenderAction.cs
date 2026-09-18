public class SetShouldDamageDefenderAction : EffectAction<EffectContext<CreatureCard>, CreatureCombatDamageEventArgs, CreatureCard> {
    private bool shouldDamageDefender;

    public SetShouldDamageDefenderAction(bool shouldDamageDefender) {
        this.shouldDamageDefender = shouldDamageDefender;
    }

    public void Execute(EffectContext<CreatureCard> context, CreatureCombatDamageEventArgs args) {
        args.ShouldDamageDefender = shouldDamageDefender;
    }
}
