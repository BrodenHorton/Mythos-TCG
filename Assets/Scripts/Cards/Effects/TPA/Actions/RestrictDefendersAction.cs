public class RestrictDefendersAction : EffectAction<EffectContext<CreatureCard>, CanDefendEventArgs, CreatureCard> {
    public void Execute(EffectContext<CreatureCard> context, CanDefendEventArgs args) {
        CanDefendEventArgs elusiveEffectArgs = new CanDefendEventArgs(args.InitiatorId,
                                                                      args.TargetId,
                                                                      args.Attacker,
                                                                      args.Defender,
                                                                      false);
        EventBus.Instance.InvokeOnSelectElusiveAttackerToDefend(elusiveEffectArgs);
        args.CanDefend = elusiveEffectArgs.CanDefend;
    }
}
