public class WitherProkedAction : EffectAction<WitherStatusContext, CreatureCombatDamageEventArgs, CreatureCard> {
    public void Execute(WitherStatusContext context, CreatureCombatDamageEventArgs args) {
        args.IsCanceled = true;
        context.WitherProkCount += args.Damage;
        TcgLogger.Log("Wither Status Proked. Count: " + context.WitherProkCount);
        EventBus.Instance.InvokeOnCreatureCardEffectClientpdate(new CreatureCardPayload(context.Card));
        context.Card.CheckHealthState();
    }
}