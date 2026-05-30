public class CreatureCombatDamageEventArgs : CreatureCombatEventArgs {
    private int damage;
    private int directDamage;
    private bool shouldDamageDefender;
    private bool isCanceled;

    public CreatureCombatDamageEventArgs(ulong initiatorId, ulong targetId, CreatureCombat creatureCombat, int damage)
        : this(initiatorId, targetId, creatureCombat.Attacker, creatureCombat.Defender, damage) { }

    public CreatureCombatDamageEventArgs(ulong initiator,
                                         ulong target,
                                         CreatureCard attacker,
                                         CreatureCard defender,
                                         int damage) : base(initiator, target, attacker, defender) {
        this.damage = damage;
        directDamage = 0;
        isCanceled = false;
        shouldDamageDefender = true;
    }

    public int Damage { get { return damage; } set { damage = value; } }

    public int DirectDamage { get { return directDamage; } set { directDamage = value; } }

    public bool ShouldDamageDefender { get { return shouldDamageDefender; } set { shouldDamageDefender = value; } } 

    public bool IsCanceled { get { return isCanceled; } set { isCanceled = value; } }
}