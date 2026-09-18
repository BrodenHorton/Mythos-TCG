public class LifelinkAction : EffectAction<EffectContext<CreatureCard>, CreatureCombatDamageEventArgs, CreatureCard> {
    private DuelManager duelManager;

    public LifelinkAction() {
        duelManager = ServiceLocator.Get<DuelManager>();
    }

    public void Execute(EffectContext<CreatureCard> context, CreatureCombatDamageEventArgs args) {
        duelManager.GetPlayerById(args.InitiatorId).ModifyLifePoints(args.Damage);
    }
}
