public class CreatureCombat {
    private CreatureCard attacker;
    private CreatureCard defender;

    public CreatureCombat(CreatureCard attacker) {
        this.attacker = attacker;
        defender = null;
    }

    public CreatureCombat(CreatureCard attacker, CreatureCard defender) {
        this.attacker = attacker;
        this.defender = defender;
    }

    public CreatureCard Attacker { get { return attacker; } }

    public CreatureCard Defender { get { return defender; } set { defender = value; } }
}