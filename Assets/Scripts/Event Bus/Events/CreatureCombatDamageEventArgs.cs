public class CreatureCombatDamageEventArgs : CreatureCombatEventArgs {
    private int damage;
    private bool isCanceled;

    public CreatureCombatDamageEventArgs(ulong initiatorId, ulong targetId, CreatureCombat creatureCombat, ref int damage)
        : this(initiatorId, targetId, creatureCombat.Attacker, creatureCombat.Defender, ref damage) { }

    public CreatureCombatDamageEventArgs(ulong initiator,
                                         ulong target,
                                         CreatureCard attacker,
                                         CreatureCard defender,
                                         ref int damage) : base(initiator, target, attacker, defender) {
        this.damage = damage;
        isCanceled = false;
    }

    public int Damage { get { return damage; } set { damage = value; } }

    public bool IsCanceled { get { return isCanceled; } set { isCanceled = value; } }
}