public class CanDefendEventArgs : CreatureCombatEventArgs {
    private bool canDefend;

    public CanDefendEventArgs(ulong initiatorId,
                              ulong targetId,
                              CreatureCard attacker,
                              CreatureCard defender) : base(initiatorId, targetId, attacker, defender) {
        canDefend = true;
    }

    public CanDefendEventArgs(ulong initiatorId,
                              ulong targetId,
                              CreatureCard attacker,
                              CreatureCard defender,
                              bool canDefend) : base(initiatorId, targetId, attacker, defender) {
        this.canDefend = canDefend;
    }

    public bool CanDefend { get { return canDefend; } set { canDefend = value; } }
}