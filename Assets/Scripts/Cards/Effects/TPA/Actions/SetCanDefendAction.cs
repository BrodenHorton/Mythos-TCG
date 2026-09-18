public class SetCanDefendAction : EffectAction<EffectContext<CreatureCard>, CanDefendEventArgs, CreatureCard> {
    private bool canDefend;

    public SetCanDefendAction(bool canDefend) {
        this.canDefend = canDefend;
    }
    
    public void Execute(EffectContext<CreatureCard> context, CanDefendEventArgs args) {
        args.CanDefend = canDefend;
    }
}
