public class CreatureDamagedByCreatureEventArgs : CreatureAttackEventArgs {
    private int damage;
    private bool isCanceled;

    public CreatureDamagedByCreatureEventArgs(MatchPlayer initiator, MatchPlayer target, CreatureCombat creatureCombat, int damage)
        : this(initiator, target, creatureCombat.Attacker, creatureCombat.Defender, damage) { }

    public CreatureDamagedByCreatureEventArgs(MatchPlayer initiator,
                                              MatchPlayer target,
                                              CreatureCard attacker,
                                              CreatureCard defender,
                                              int damage) : base(initiator, target, attacker, defender) {
        this.damage = damage;
        isCanceled = false;
    }

    public int Damage { get { return damage; } set { damage = value; } }

    public bool IsCanceled { get { return isCanceled; } set { isCanceled = value; } }

}