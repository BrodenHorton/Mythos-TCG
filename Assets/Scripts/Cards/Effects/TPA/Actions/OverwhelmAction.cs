public class OverwhelmAction : EffectAction<EffectContext<CreatureCard>, CreatureCombatDamageEventArgs, CreatureCard> {
    public void Execute(EffectContext<CreatureCard> _, CreatureCombatDamageEventArgs args) {
        TcgLogger.Log("Overwhelm Effect triggered");
        int overwhelmDamage = 0;
        if (args.Defender == null)
            overwhelmDamage = args.Damage;
        else if (args.Defender.GetHealth() < args.Damage)
            overwhelmDamage = args.Damage - args.Defender.GetHealth();

        if (overwhelmDamage > 0)
            args.DirectDamage = overwhelmDamage;
    }
}