public class CreatureDamagedByCreatureEventArgs : CreatureCombatEventArgs {
    private int damage;
    private bool isCanceled;

    public CreatureDamagedByCreatureEventArgs(ulong initiatorId, ulong targetId, CreatureCombat creatureCombat, int damage)
        : this(initiatorId, targetId, creatureCombat.Attacker, creatureCombat.Defender, damage) { }

    public CreatureDamagedByCreatureEventArgs(ulong initiator,
                                              ulong target,
                                              CreatureCard attacker,
                                              CreatureCard defender,
                                              int damage) : base(initiator, target, attacker, defender) {
        this.damage = damage;
        isCanceled = false;
    }

    public int Damage { get { return damage; } set { damage = value; } }

    public bool IsCanceled { get { return isCanceled; } set { isCanceled = value; } }
}